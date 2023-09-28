# Colibri

This repo contains the Colibri core runtime (which includes the standard library) and the Colibri Interpreter and REPL.

## What is Colibri?

Colibri is a Lisp-based language that is written in C# (with some library functions written in Colibri itself) and runs on [.NET](https://dotnet.microsoft.com/). 
This project was formerly known as Lillisp, which has since been archived, prior to the introduction of the modern syntax.

Currently, Colibri can call some .NET code, but anything defined in it is not yet easily callable *from* .NET code. Colibri can be used as a REPL, or you can specify a file to interpret. Compilation is on the roadmap but not yet supported.

Colibri is a Scheme-based Lisp, and ultimately aims to be as [R7RS-small](https://small.r7rs.org/) compliant as possible. Being a Scheme, Colibri is a [Lisp-1](https://andersmurphy.com/2019/03/08/lisp-1-vs-lisp-2.html), meaning functions and variables/parameters cannot share the same name, and functions do not need to be quoted to be passed as values.

Colibri also draws inspiration from Clojure, and uses its syntax in part, such as with .NET interop.

Colibri started as a C# implementation of Peter Norvig's lis.py from the blog post [(How to Write a (Lisp) Interpreter (in Python))](https://norvig.com/lispy.html). Many thanks to Peter for the excellent tutorial that inspired this project.

## Colibri Syntax

Colibri is a Scheme-based language, so most Scheme code should work. If not, please file an issue!

Where this project differs from my former Lillisp project is primarily around the introduction of a new syntax. 
This syntax was inspired by [Lisp 2](https://en.wikipedia.org/wiki/LISP_2), but with far less ambitious goals: reduce parentheses, and add type annotations.

For a quick demo, compare this Scheme code (which works in Colibri as well):
```lisp
(define (square x) (* x x))
(square 4)
```

To the following equivalent Colibri syntax:
```python
define (square x) (* x x)
square 4
```

Notice that parentheses are not required for top-level statements that have at least one argument, and it is newline-sensitive.

Colibri goes further with a `fn` alias for the `defun` macro for a Rust-like syntax. 
Note that this is still Lisp and S-Expressions, just with some rules around when you are allowed to drop parentheses:

```rust
fn square (x) (* x x)
square 4
```

But what if you need to define a function with multiple statements/lines in its body? 
Colibri has you covered with an additional syntax mechanism: Statement Lists.
Statement Lists are lists that use curly braces instead of parentheses, are newline-sensitive (just like top-level code above), and follow the same rules around parentheses relaxation. Here's an example with multiple statements in a function:

```rust
fn sayHello (name) {
    display "Well, hello, "
    display name
    (newline)
}

sayHello "Paul"
```

Note again that the newline call requires parentheses, because it does not have an argument, and thus we wouldn't know whether you intend to call it, or yield that delegate as an expression.

The code above is effectively identical in the abstract syntax tree to the following (which, of course, still works in Colibri):
```lisp
(fn sayHello (name)
    (display "Well, hello, ")
    (display name)
    (newline)
)

(sayHello "Paul")
```

You also can use semicolons when in statement mode (either in Statement Lists or at the top level of your code) to separate statements without having to use parentheses. i.e. the prior function could be written in one line as:

```rust
fn sayHello (name) { display "Well, hello, "; display name; (newline) }
```

> *NOTE*: The following section is a work in progress and represents my thoughts about how this feature might work.
> It might not necessarily be fully implemented or complete at this time. Please take this with a grain of salt.

The third new syntax feature is what Colibri calls Adaptive Collections. 

Adaptive Collections can take on three forms:
* Lists of items, such as `[ 1, 2, 3 ]`, which can become vectors or .NET arrays/lists
* Associative Arrays of key/value pairs, such as `[ "foo": 1, "bar": 2 ]` or `[ x: 1, y: 2 ]`
* List Comprehensions using LispINQ, such as `[ from x in items where (= (% x 2) 0) select x ]`

Similar to the [C# 12 collection expressions feature](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions),
the List form of Adaptive Collections can masquerade as any collection-like type in Colibri.
It implements the common `IEnumerable`/`ICollection`/`IList`/etc. .NET collection interfaces, 
as well as it is implicitly convertible to a Scheme list, Scheme vector, `List<object?>`, `object?[]`, `HashSet<object?>`, etc.

The Associative Array form can likewise masquerade as any dictionary-like type.
It implements `IEnumerable<KeyValuePair<object, object?>>` and `IDictionary<object, object?>`,
as well as it is implicitly convertible to types like `List<KeyValuePair<object, object?>>` and `Dictionary<object, object?>`.
Associative Arrays are also implicitly convertible to Scheme lists of lists, or what we call Pairwise Lists. 
For example, `[x: 1, y: 2]` is convertible to `((x 1) (y 2))`.
Additionally, Associative Arrays are `dynamic`, so they support member access as if they were an object, if the keys are symbols.

In both cases above, commas are treated like whitespace, just like in Clojure. 
They can help with readability but are not strictly required.
If you're more comfortable writing `[ 1 2 3 ]` than `[ 1, 2, 3 ]` then you can do just that.

Finally, the List Comprehension form is similar to Python's list comprehensions, but with a LINQ syntax.
See the LispINQ section below for more details.
The List Comprehension form is equivalent to calling `.ToList()` on a LINQ query, except that it also has all
of the benefits of the List form of Adaptive Collections above.

The associative array form can be used to make `let` a bit less parentheses-heavy:
```rust
let* [
    x: 1,
    y: (+ x 2)
] {
    display y
    (newline)
}
```

Note that associative arrays by default are *not* hashmaps/dictionaries. 
This allows for retaining insertion order if desired. 

For example:
```rust
define items [ z: 99, y: 98, x: 97]
.Value (get items 0) // yields 99
```

The `dynamic` aspect of associative arrays allows for easy (if not relatively slow) anonymous object creation:
```rust
define items [ 
    [ id: 1, name: "Lisp" ], 
    [ id: 2, name: "Scheme" ] 
]

// yields "Scheme"
.name (get items 1)
```

However, associative arrays require unique non-null keys, so they do not support multiple values per key.

Finally, Colibri, being a .NET language, is in the early phases of adding type annotations. 
These type annotations are currently only checked at runtime, but there are plans in the near future to support exporting Colibri functions in assemblies with real .NET types. 
Also note that currently only some basic types are supported; complex types are not yet implemented.

In the example below, the `:` after the parameter name is just syntactic sugar; it is ignored. 
Likewise, the `->` is just a symbol that is solely interpreted by the `fn`/`defun` macro to indicate that the subsequent argument is a return type.

Example:
```rust
fn square (x: i32) -> i32 {
    * x x
}

square "foo" // ERROR: Expected type System.Int32 for argument x but got System.String.
```

Built-in type "keywords" are just values in the global scope that resolve to `System.Type` objects, so you can feel free to use them without needing a `typeof` operator. 
Colibri has the following type aliases defined:

| Alias | Type |
| --- | --- |
| `i8` | System.SByte |
| `i16` | System.Int16 |
| `i32` | System.Int32 |
| `i64` | System.Int64 |
| `u8` | System.Byte |
| `u16` | System.UInt16 |
| `u32` | System.UInt32 |
| `u64` | System.UInt64 |
| `f32` | System.Single |
| `f64` | System.Double |
| `char` | System.Char |
| `str` | System.String |
| `bool` | System.Boolean |
| `void` | System.Void |
| `dec` | System.Decimal |
| `obj` | System.Object |

You can also indicate if a type should be a list (of any length and element type) with `(...)`, or nil with `()`. 
The latter is useful for the return type of "void"-returning functions.

Future enhancements will likely expand this type syntax to support .NET generic types, complex types, pattern matching, and so on.

Please note that type checking in Colibri is _extremely early_ and is likely currently only useful for the most trivial of examples.

### Colibri Syntax Ideas

Here are some ideas for the future:

* Associative Array Comprehension syntax, i.e. `[ from x in values select (.Id x): (.Name x) ]`
* Strongly-typed Adaptive Collections
  * Lists, i.e. `[<str> "foo", "bar"]` or `<str>[ "foo", "bar" ]`
  * Associative Arrays, i.e. `[<Symbol, i32> x: 1, y: 2]` or `<Symbol, i32>[ x: 1, y: 2]`
  * List comprehensions, i.e. `[<u8> from x in values select (% x 255)]`
* Anonymous types, i.e. `#[ x: a, y: b ]`

## Using the REPL

Build and run the Colibri project in Visual Studio 2022 or later, or via the `dotnet` CLI.

Input commands and hit enter. Note that multi-line input is not yet supported.

Certain REPL commands may be entered, without parentheses. These include:

| Command | Description |
| --- | --- |
| `clear` or `cls` | Clears the console buffer/window and starts a new input line at the top of the buffer. |
| `exit` or `quit` or <kbd>Ctrl</kbd>+<kbd>C</kbd> | Exits the Colibri REPL. |
| `reset` | Resets the current Colibri runtime environment, starting fresh. All data in memory will be lost. |

## Colibri CLI

Running the Colibri project executable will launch the REPL if no command-line arguments are provided.

The following command-line arguments are supported:

| Argument | Description |
| --- | --- |
| `--file <path>` | Run the Colibri interpreter on the specified file. |
| `--version` | Display the Colibri version. |
| `--help` | Display help information on the available command-line arguments. |

## The Colibri Language

Full docs coming at some point in the future. Check out StandardLibraries.cs for the included batteries.

An incomplete list of features currently supported:
* Data types: list, pair, vector, bytevector, number, boolean, character, string, symbol, nil, procedure
* Number types: complex (rectangular `-3+2i` notation), real, rational (i.e. `3/8`), integer
* Defining variables with `define` (aliased as `def`)
* Mutating variables with `set!`
* Most common math operations (`+`, `-`, `*`, `/`, `abs`, `log`, `sqrt`, etc. - others available in `System.Math`)
* Boolean expressions (`<`, `>`, `<=`, `==`, `and`, `or`, etc)
* List operations (`list`, `car`, `cdr`, `cons`, etc)
* Quoting expressions (i.e. `'(1 2 3)` or `(quote (1 2 3))`)
* Quasiquoting, unquoting, and splicing (i.e. ``(eval `(+ ,@(range 0 10)))``)
* Higher-order list functions (`apply`, `map`)
* Conditional logic (`if`, `cond`, `when`)
* Sequential logic with `begin`
* Lambda expressions with `lambda`
* Tail recursion (except for any invoked non-Colibri .NET code)
* Shorthand for defining a lambda variable (aka a named function) with `defun` (or `define` with a list as the first parameter)
* Block-scoping variables with `let`, `let*`, etc
* Rational number operations (`rationalize`, `numerator`/`denominator`, `simplify`)
* Exceptions (`with-exception-handler`, `raise`, `error`, `raise-continuable`, etc.)
* Record types with `define-record-type`
* Macros with `define-syntax` and `syntax-rules` (support might be incomplete; please file an issue with example if you find something that fails)
* Importing standard Scheme libraries with `import` (by default, all standard libraries are automatically imported)
* Almost all of the Scheme `base`, `char`, `complex`, `CxR`, `eval`, `file`, `inexact`, `lazy`, `load`, `process-context`, `read`, `repl`, `time`, and `write` library functions

Notable features not yet fully implemented from Scheme R7RS-small include:
* Defining your own libraries with `define-library`
* `#!fold-case` (and friends) and `include-ci`
* The R5RS library

Basically, give your existing Scheme code a try, and if a given feature doesn't work, file an issue.

## .NET Interop

Colibri uses Clojure-inspired syntax for interop with .NET types. Any .NET object can be stored to a Colibri variable. (Note: all values are boxed to `System.Object`.)

.NET Interop is *extremely experimental* and *very fragile*.

### Static Methods

You can call static methods on types with the `/` character in place of i.e. a `.` character in C#. Examples:

```lisp
>>> (String/IsNullOrWhiteSpace "foo")
-> False
>>> (String/IsNullOrWhiteSpace " \t ")
-> True
>>> (Int32/Parse "123")
-> 123
>>> (Guid/NewGuid)
-> 7bf62a3c-bcd1-4e38-aceb-50f13c4113d5
```

### Static Members

Just like static methods, you can access static members like fields and properties the same way.

```lisp
>>> Int32/MaxValue
-> 2147483647
>>> (def intmax Int32/MaxValue)
-> intmax
>>> intmax
-> 2147483647
```

### Creating Objects

.NET objects can be created with the `new` function. The first parameter is the .NET type name, followed by any constructor parameters.

```lisp
>>> (new Object)
-> System.Object
>>> (def rnd (new Random))
-> rnd
>>> rnd
-> System.Random
```

### Instance Methods

Just like Clojure, you can call instance methods on an object with the name of the method proceeded by a `.`, then the instance to call it on, followed by any parameters.

```lisp
>>> (def rnd (new Random))
-> rnd
>>> (.Next rnd)
-> 1055373556
>>> (.Next rnd)
-> 938800480
>>> (.Next rnd 100)
-> 73
>>> (.Next rnd 100)
-> 55
```

### Instance Members

Likewise, instance members (fields and properties) can be accessed the same way.

```lisp
>>> (def u (new Uri "https://www.github.com/paulirwin/lillisp"))
-> u
>>> (.Scheme u)
-> "https"
>>> (.Host u)
-> "www.github.com"
>>> (.PathAndQuery u)
-> "/paulirwin/lillisp"
```

### Importing Namespaces

Currently, only the .NET 6 Base Class Library is available. Any namespaces can be "imported" (like the `using` statement in C#) into the current environment with the `use` keyword and a quoted symbol of the namespace name. Examples:

```
>>> (new StringBuilder)
ERROR: Unable to resolve symbol StringBuilder
>>> (use 'System.Text)
-> ()
>>> (def x (new StringBuilder))
-> x
>>> x
-> ""
>>> (.Append x #\*)
-> "*"
>>> (.Append x "foo")
-> "*foo"
>>> (.AppendLine x "bar!")
-> "*foobar!\r\n"
```

### Generic Types

Generic types can be used as if invoking the type as a function. In other languages, generic types are called "parameterized types", so Colibri takes this literally, as if parameters to a function. Think of an "open" generic type as a function that takes type parameters and returns a "closed" generic type. For example, `List<String>` becomes `(List String)`.

```lisp
>>> (use 'System.Collections.Generic)
-> ()
>>> (List String)
-> System.Collections.Generic.List`1[System.String]
>>> (Dictionary Int32 Guid)
-> System.Collections.Generic.Dictionary`2[System.Int32,System.Guid]
>>> (def x (new (List String)))
-> x
>>> (.Add x "foo")
-> null
>>> (.Add x "bar")
-> null
>>> x
-> ("foo" "bar")
```

Note that generic methods are not yet supported.

### .NET Types

All variables are boxed to `System.Object`.

You can get the type of a variable with either `(.GetType var)` or `(typeof var)`. Notice how `typeof` operates more like JavaScript than C#. 
There is no need to use a keyword to get a .NET `System.Type` reference: just use the type name directly. This enables convenient use of the `new` function. 
This also potentially enables interesting runtime reflection scenarios, such as using `new` with a variable of the type to instantiate, without a bunch of `Activator.CreateInstance` boilerplate.

You can also cast using the `cast` function. Examples:

```lisp
>>> (.GetType "foo")
-> System.String
>>> (typeof "foo")
-> System.String
>>> Int32
-> System.Int32
>>> (.GetType Int32)
-> System.RuntimeType
>>> (def t Uri)
-> t
>>> (new t "https://www.google.com")
-> https://www.google.com/
>>> (typeof 7)
-> System.Double
>>> (typeof (cast 7 Int32))
-> System.Int32
```

Common Colibri to .NET type mappings:
| Colibri type | .NET type |
| --- | --- |
| list/pair | `Colibri.Core.Pair` |
| vector | `Colibri.Core.Vector` (wraps a `System.Collections.Generic.List<System.Object?>`) |
| bytevector | `Colibri.Core.Bytevector` (wraps a `System.Collections.Generic.List<System.Byte>`) |
| boolean (i.e. `true` or `#t`) | `System.Boolean` |
| integer numbers (i.e. `7`) | `System.Int32` |
| real numbers (i.e. `42.03` or `1.1e-10`) | `System.Double` |
| rational numbers (i.e. `3/8`) | [`Rationals.Rational`](https://github.com/tompazourek/Rationals) |
| complex numbers (i.e. `-4+7i`) | `System.Numerics.Complex` |
| character | `System.Char` |
| constant string | `System.String` |
| mutable string (i.e. with `(make-string)`) | `System.Text.StringBuilder` |

## Creating Types

C#-like "record" types can be defined with the `defrecord` function. 
Note that these are true .NET class types, and are not to be confused with Scheme records.

The `defrecord` form is: `(defrecord TypeName *properties)`

`*properties` is one or more property definitions. A property definition is either of the form `Name` (for an `object`-typed property) or `(Name Type)` where `Type` is a .NET type name.

Example: `(defrecord Customer (Id Int32) (Name String))`

Each property is generated on the new record type with a private backing field, and a constructor parameter in the order specified. 
Properties without a type specified will be of type `System.Object`.

In addition, a `ToString` implementation is generated, along with equality members. Two records with the same values will not be `eq?` (aka reference equals)
but will be `eqv?` (aka value-wise equivalent).

Records are IL-emitted at runtime, when the type is defined. This means they are real .NET types, and so you can create a `(List Customer)` that is strongly-generic-typed to the newly-created `Customer` type.

```lisp
>>> (defrecord Customer (Id Int32) (Name String) (Balance Double))
-> Customer
>>> (new Customer 123 "Foo Bar" 2021.11)
-> Customer { Id = 123, Name = Foo Bar, Balance = 2021.11 }
>>> (def c (new Customer 234 "Fizz Buzz" 123.45))
-> c
>>> (.Id c)
-> 234
>>> (.Name c)
-> "Fizz Buzz"
>>> (def c2 (new Customer 234 "Fizz Buzz" 123.45))
-> c2
>>> (eq? c c2)
-> False
>>> (eqv? c c2)
-> True
```

You can also create .NET enum tyes with `defenum`. The first argument is the enum type name, followed by its values. Just like records, this is a real .NET enum type, dynamically IL-generated at runtime. 

Example:

```lisp
>>> (defenum Fruit Apple Banana Cranberry Date Elderberry)
-> Fruit
>>> Fruit/Apple
-> Apple
>>> (def myfruit Fruit/Apple)
-> myfruit
>>> (str myfruit)
-> "Apple"
>>> (eqv? myfruit Fruit/Apple)
-> True
>>> (eqv? myfruit Fruit/Banana)
-> False
>>> (Enum/GetValues Fruit)
-> (Apple Banana Cranberry Date Elderberry)
>>> (cast 4 Fruit)
-> Elderberry
>>> (cast Fruit/Cranberry Int32)
-> 2
```

## LispINQ

Work has started on adding a LINQ-like syntax to Colibri, called LispINQ. Currently `from`, `where`, `orderby`, `thenby`, and `select` are supported to some degree.

This is probably best expressed with an example:

```lisp
>>> (defrecord Customer (Id Int32) (Name String))
-> Customer
>>> (use 'System.Linq)
-> ()
>>> (use 'System.Collections.Generic)
-> ()
>>> (def mylist (new (List Customer)))
-> mylist
>>> (.Add mylist (new Customer 123 "Foo Bar"))
-> null
>>> (.Add mylist (new Customer 234 "Fizz Buzz"))
-> null
>>> (.Add mylist (new Customer 345 "Buzz Lightyear"))
-> null
>>> (.ToArray (from i in mylist where (.StartsWith (.Name i) "F") orderby (.Id i) desc select (.Id i)))
-> (234 123)
>>> (.ToArray (from i in mylist where (eqv? (% (.Id i) 2) 1) orderby (.Id i) desc select (.Id i)))
-> (345 123)
```

Note that the example above also makes use of dynamic dispatch of extension methods, with the `.ToArray` call. 
And as you can see in the last line, you can mix and match Colibri expressions with .NET interop in LispINQ expressions.

Unlike C#, to add an additional `orderby` clause, you'll add a `thenby` for any subsequent ordering expressions.
e.g.: `(from i in mylist orderby (.Name i) thenby (.Id i) desc select i)`. Also note that LispINQ uses `desc` to indicate descending, while C# uses `descending`.
