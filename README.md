# Colibri

This repo contains the Colibri core runtime (which includes the standard library) and the Colibri REPL.

## What is Colibri?

Colibri (prounounced "lill-isp", like "lillies") is a Lisp-based language that is written in C# (with some library functions written in Colibri itself) and runs on [.NET](https://dotnet.microsoft.com/). 
This project was formerly known as Lillisp, which has since been archived, prior to the introduction of the modern syntax.

Currently, Colibri can call some .NET code, but anything defined in it is not yet easily callable *from* .NET code. Colibri can be used as a REPL, or you can specify a file to interpret. Compilation is on the roadmap but not yet supported.

Colibri is a Scheme-based Lisp, and ultimately aims to be as [R7RS-small](https://small.r7rs.org/) compliant as possible. Being a Scheme, Colibri is a [Lisp-1](https://andersmurphy.com/2019/03/08/lisp-1-vs-lisp-2.html), meaning functions and variables/parameters cannot share the same name, and functions do not need to be quoted to be passed as values.

Colibri also draws inspiration from Clojure, and uses its syntax in part, such as with .NET interop.

Colibri started as a C# implementation of Peter Norvig's lis.py from the blog post [(How to Write a (Lisp) Interpreter (in Python))](https://norvig.com/lispy.html). Many thanks to Peter for the excellent tutorial that inspired this project.

## Screenshot

![image](https://user-images.githubusercontent.com/1874103/137605342-15623f3f-9ca0-429c-b655-e02176ba6b9a.png)

(Using the open-source [Windows Terminal](https://github.com/microsoft/terminal), [Powershell 7](https://github.com/PowerShell/PowerShell), and [Cascadia Code](https://github.com/microsoft/cascadia-code) font, running on .NET 6.)

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

Full docs coming at some point in the future. But it's basically a normal Lisp with mostly Scheme syntax. Check out ColibriRuntime.cs and Library/core.lisp for built-in library methods.

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
* Tail recursion (except for `and` and `or`, or any invoked .NET code)
* Shorthand for defining a lambda variable (aka a named function) with `defun` (or `define` with a list as the first parameter)
* Block-scoping variables with `let`, `let*`, etc
* Rational number operations (`rationalize`, `numerator`/`denominator`, `simplify`)
* Exceptions (`with-exception-handler`, `raise`, `error`, `raise-continuable`, etc.)
* Record types with `define-record-type`
* Almost all of the Scheme base library string-, vector-, port-, and bytevector-related functions
* Almost all of the Scheme `char`, `complex`, `CxR`, `file`, `inexact`, `lazy`, `process-context`, `read`, `time`, and `write` library functions

Notable features not yet implemented from Scheme R7RS include:
* Tail context for `and` and `or`
* Macros
* Libraries (as in, i.e. `import`)
* Many base library methods, and other libraries

Basically, give your existing Lisp code a try, and if a given feature doesn't work, file an issue.

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