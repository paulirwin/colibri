parser grammar ColibriParser;

options { tokenVocab = ColibriLexer;
}

prog: form * EOF;

form: regex | atom | list | bytevector | vector | meta;

atom: (number | symbol | STRING | CHARACTER);

list: LPAREN form* RPAREN;

bytevector: BYTEVECTOR_PREFIX integer* RPAREN;

vector: LBRACKET form* RBRACKET | VECTOR_PREFIX form* RPAREN;

meta: quote | quasiquote | unquote_splicing | unquote | comment_datum;

integer: INTEGER;

quote: SINGLE_QUOTE form;

quasiquote: BACKTICK form;

unquote_splicing: COMMA_AT form;

unquote: COMMA form;

comment_datum: HASH_SEMICOLON form;

number: prefixed_number | INTEGER | FLOAT | COMPLEX | RATIO | POS_INFINITY | NEG_INFINITY | NAN;

prefixed_number: hex_prefixed | decimal_prefixed | octal_prefixed | binary_prefixed;

hex_prefixed: HEX_PREFIXED_NUMBER;

octal_prefixed: OCTAL_PREFIXED_NUMBER;

binary_prefixed: BINARY_PREFIXED_NUMBER;

decimal_prefixed: DECIMAL_PREFIX (INTEGER | FLOAT);

regex: REGEX_PATTERN IDENTIFIER?;

symbol: DOT_LITERAL | ELLIPSIS_LITERAL | UNDERSCORE_LITERAL | BOOLEAN | IDENTIFIER | ESCAPED_IDENTIFIER;
