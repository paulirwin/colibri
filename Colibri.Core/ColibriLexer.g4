lexer grammar ColibriLexer;

LPAREN: '(' -> pushMode(ParenMode);
RPAREN: ')' -> popMode;

// Statement block support
// See the parser file for more information
LCURLY: '{' -> pushMode(DEFAULT_MODE);
RCURLY: '}' -> popMode;

// Pairwise block support
// See the parser file for more information
LBRACKET: '[' -> pushMode(DEFAULT_MODE);
RBRACKET: ']' -> popMode;

SINGLE_QUOTE: '\'';
BACKTICK: '`';
COMMA_AT: ',@';
COMMA: ',';
HASH_SEMICOLON: '#;';
SEMICOLON: ';';

POS_INFINITY: '+inf.0';
NEG_INFINITY: '-inf.0';
NAN: '+nan.0' | '-nan.0';
BOOLEAN: '#t' | '#f';

DOT_LITERAL: '.';
ELLIPSIS_LITERAL: '...';
UNDERSCORE_LITERAL: '_';

BYTEVECTOR_PREFIX: '#u8(' -> pushMode(ParenMode);
VECTOR_PREFIX: '#(' -> pushMode(ParenMode);
HEX_PREFIXED_NUMBER: HEX_PREFIX (HEX_DIGIT | UNDERSCORE)+;
OCTAL_PREFIXED_NUMBER: OCTAL_PREFIX (OCTAL_DIGIT | UNDERSCORE)+;
BINARY_PREFIXED_NUMBER: BINARY_PREFIX (BINARY_DIGIT | UNDERSCORE)+;

INTEGER: NEGATE? (DIGIT | UNDERSCORE)+;
COMPLEX: ((NEGATE? (DIGIT | UNDERSCORE | '.')+) | POS_INFINITY | NEG_INFINITY | NAN) ('+' | '-') ((DIGIT | UNDERSCORE | '.')+ | 'inf.0' | 'nan.0') 'i';
FLOAT: NEGATE? (DIGIT | UNDERSCORE | '.')+ ('e' '-'? (DIGIT | UNDERSCORE | '.')+)?;
RATIO: NEGATE? (DIGIT | UNDERSCORE)+ '/' (DIGIT | UNDERSCORE)+;

CHARACTER: '#\\' ((LETTER | DIGIT | SYMBOL_CHAR)* | '(' | ')');

REGEX_PATTERN: '/' ( ~('/' | ' ') | '\\' '/' | '\\' ' ' )* '/' REGEX_FLAGS;

ESCAPED_IDENTIFIER: '|' ( ~'|' | '\\' '|' )* '|';

IDENTIFIER: IDENTIFIER_START IDENTIFIER_PART*;

STRING: '"' ( ~'"' | '\\' '"' )* '"' ;

fragment REGEX_FLAGS: IDENTIFIER_PART*;

fragment IDENTIFIER_START: (LETTER | SYMBOL_CHAR);
fragment IDENTIFIER_PART: (LETTER | DIGIT | SYMBOL_CHAR);

fragment HEX_PREFIX: '#x';
fragment OCTAL_PREFIX: '#o';
fragment BINARY_PREFIX: '#b';
DECIMAL_PREFIX: '#de' | '#di' | '#ed' | '#id' | '#d' | '#e' | '#i';

fragment HEX_DIGIT: '0'..'9' | 'a'..'f' | 'A'..'F';
fragment DIGIT: '0'..'9';
fragment OCTAL_DIGIT: '0'..'7';
fragment BINARY_DIGIT: '0' | '1';

fragment LETTER: LOWER | UPPER;
fragment LOWER: 'a'..'z';
fragment UPPER: 'A'..'Z';

fragment SYMBOL_CHAR:
	'+'
	| '-'
	| '*'
	| '/'
	| '%'
	| '^'
	| '<'
	| '>'
	| '='
	| '!'
	| '&'
	| '|'
	| '$'
	| '.'
	| ':'
	| '?'
	| '@'
	| '~'
	| '_';
fragment NEGATE: '-';
fragment UNDERSCORE: '_';

BLOCK_COMMENT: '/*' .*? '*/' -> channel(HIDDEN);
BLOCK_COMMENT_ALT: '#|' .*? '|#' -> channel(HIDDEN);
LINE_COMMENT: '//' ~[\r\n]* -> channel(HIDDEN);

WHITESPACE: [ \t]+ -> channel(HIDDEN);
NEWLINE: '\r'? '\n';

mode ParenMode;
    PAREN_WHITESPACE: [ \r\t]+ -> channel(HIDDEN);
    PAREN_NEWLINE: NEWLINE -> type(NEWLINE), channel(HIDDEN);
    
    PAREN_LCURLY: LCURLY -> type(LCURLY), pushMode(DEFAULT_MODE);
    PAREN_RCURLY: RCURLY -> type(RCURLY), popMode;
    PAREN_LPAREN: LPAREN -> type(LPAREN), pushMode(ParenMode);
    PAREN_RPAREN: RPAREN -> type(RPAREN), popMode;
    PAREN_LBRACKET: LBRACKET -> type(LBRACKET), pushMode(DEFAULT_MODE);
    PAREN_RBRACKET: RBRACKET -> type(RBRACKET), popMode;
    
    PAREN_SINGLE_QUOTE: SINGLE_QUOTE -> type(SINGLE_QUOTE);
    PAREN_BACKTICK: BACKTICK -> type(BACKTICK);
    PAREN_COMMA_AT: COMMA_AT -> type(COMMA_AT);
    PAREN_COMMA: COMMA -> type(COMMA);
    PAREN_HASH_SEMICOLON: HASH_SEMICOLON -> type(HASH_SEMICOLON);
    PAREN_SEMICOLON: SEMICOLON -> type(SEMICOLON);
    
    PAREN_POS_INFINITY: POS_INFINITY -> type(POS_INFINITY);
    PAREN_NEG_INFINITY: NEG_INFINITY -> type(NEG_INFINITY);
    PAREN_NAN: NAN -> type(NAN);
    PAREN_BOOLEAN: BOOLEAN -> type(BOOLEAN);
    PAREN_DOT_LITERAL: DOT_LITERAL -> type(DOT_LITERAL);
    PAREN_ELLIPSIS_LITERAL: ELLIPSIS_LITERAL -> type(ELLIPSIS_LITERAL);
    PAREN_UNDERSCORE_LITERAL: UNDERSCORE_LITERAL -> type(UNDERSCORE_LITERAL);
    PAREN_BYTEVECTOR_PREFIX: BYTEVECTOR_PREFIX -> type(BYTEVECTOR_PREFIX), pushMode(ParenMode);
    PAREN_VECTOR_PREFIX: VECTOR_PREFIX -> type(VECTOR_PREFIX), pushMode(ParenMode);
    PAREN_HEX_PREFIXED_NUMBER: HEX_PREFIXED_NUMBER -> type(HEX_PREFIXED_NUMBER);
    PAREN_OCTAL_PREFIXED_NUMBER: OCTAL_PREFIXED_NUMBER -> type(OCTAL_PREFIXED_NUMBER);
    PAREN_BINARY_PREFIXED_NUMBER: BINARY_PREFIXED_NUMBER -> type(BINARY_PREFIXED_NUMBER);
    PAREN_INTEGER: INTEGER -> type(INTEGER);
    PAREN_COMPLEX: COMPLEX -> type(COMPLEX);
    PAREN_FLOAT: FLOAT -> type(FLOAT);
    PAREN_RATIO: RATIO -> type(RATIO);
    PAREN_CHARACTER: CHARACTER -> type(CHARACTER);
    PAREN_REGEX_PATTERN: REGEX_PATTERN -> type(REGEX_PATTERN);
    PAREN_ESCAPED_IDENTIFIER: ESCAPED_IDENTIFIER -> type(ESCAPED_IDENTIFIER);
    PAREN_IDENTIFIER: IDENTIFIER -> type(IDENTIFIER);
    PAREN_STRING: STRING -> type(STRING);
    PAREN_DECIMAL_PREFIX: DECIMAL_PREFIX -> type(DECIMAL_PREFIX);
    PAREN_BLOCK_COMMENT: '#|' .*? '|#' -> type(BLOCK_COMMENT);
    PAREN_LINE_COMMENT: ';' ~[\r\n]* -> type(LINE_COMMENT);