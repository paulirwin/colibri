parser grammar ColibriParser;

options { tokenVocab = ColibriLexer;
}

prog: expr* EOF;

expr: regex | atom | list | bytevector | vector | meta | statementBlock | pairwiseBlock;

// Statement block support:
// A statement block is a special type of S-expression that is newline sensitive.
// Each line constitutes a statement, and multiple statements can be placed
// on a single line by separating them with semicolons.
// The last statement in a block is the return value of the block.
// Statements get interpreted by default as essentially a `begin` expression,
// with each statement automatically wrapped in parentheses if it is not already.
// i.e.: { + x 2; x } is equivalent to (begin (+ x 2) x)

statementBlock: LCURLY (statementExpr | NEWLINE)* RCURLY;

statementExpr: ((identifier expr*) | expr) (SEMICOLON | NEWLINE)*;

// Pairwise block support:
// A pairwise block is a special type of S-expression that is newline sensitive.
// Each pair of expressions in the block is turned into a pair.
// Semicolons can be used to separate pairs if desired, but are not required.
// i.e. [ x 4; y 8 ] is equivalent to ((x 4) (y 8))

pairwiseBlock: LBRACKET (pairwiseExpr | NEWLINE)* RBRACKET;

pairwiseExpr: expr expr (SEMICOLON | NEWLINE)*;

atom: (number | symbol | STRING | CHARACTER);

list: LPAREN expr* RPAREN;

bytevector: BYTEVECTOR_PREFIX integer* RPAREN;

vector: VECTOR_PREFIX expr* RPAREN;

meta: quote | quasiquote | unquote_splicing | unquote | comment_datum;

integer: INTEGER;

quote: SINGLE_QUOTE expr;

quasiquote: BACKTICK expr;

unquote_splicing: COMMA_AT expr;

unquote: COMMA expr;

comment_datum: HASH_SEMICOLON expr;

number: prefixed_number | INTEGER | FLOAT | COMPLEX | RATIO | POS_INFINITY | NEG_INFINITY | NAN;

prefixed_number: hex_prefixed | decimal_prefixed | octal_prefixed | binary_prefixed;

hex_prefixed: HEX_PREFIXED_NUMBER;

octal_prefixed: OCTAL_PREFIXED_NUMBER;

binary_prefixed: BINARY_PREFIXED_NUMBER;

decimal_prefixed: DECIMAL_PREFIX (INTEGER | FLOAT);

regex: REGEX_PATTERN IDENTIFIER?;

symbol: DOT_LITERAL | ELLIPSIS_LITERAL | UNDERSCORE_LITERAL | BOOLEAN | identifier;

identifier: IDENTIFIER | ESCAPED_IDENTIFIER;
