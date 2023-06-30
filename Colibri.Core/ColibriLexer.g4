lexer grammar ColibriLexer;

LPAREN: '(' -> pushMode(DEFAULT_MODE);
RPAREN: ')' -> popMode;

// Statement block support
// See the parser file for more information
LCURLY: '{' -> pushMode(BlockMode);
RCURLY: '}' -> popMode;

// Pairwise block support
// See the parser file for more information
LBRACKET: '[' -> pushMode(BlockMode);
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

BYTEVECTOR_PREFIX: '#u8(' -> pushMode(DEFAULT_MODE);
VECTOR_PREFIX: '#(' -> pushMode(DEFAULT_MODE);
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

BLOCK_COMMENT: '#|' .*? '|#' -> channel(HIDDEN);
LINE_COMMENT: ';' ~[\r\n]* -> channel(HIDDEN);

WHITESPACE: [ \r\t]+ -> channel(HIDDEN);
NEWLINE: '\r'? '\n' -> channel(HIDDEN);

mode BlockMode;
    STMT_WHITESPACE: [ \t]+ -> channel(HIDDEN);
    STMT_NEWLINE: NEWLINE -> type(NEWLINE);
    
    STMT_LCURLY: LCURLY -> type(LCURLY), pushMode(BlockMode);
    STMT_RCURLY: RCURLY -> type(RCURLY), popMode;
    STMT_LPAREN: LPAREN -> type(LPAREN), pushMode(DEFAULT_MODE);
    STMT_RPAREN: RPAREN -> type(RPAREN), popMode;
    STMT_LBRACKET: LBRACKET -> type(LBRACKET), pushMode(BlockMode);
    STMT_RBRACKET: RBRACKET -> type(RBRACKET), popMode;
    
    STMT_SINGLE_QUOTE: SINGLE_QUOTE -> type(SINGLE_QUOTE);
    STMT_BACKTICK: BACKTICK -> type(BACKTICK);
    STMT_COMMA_AT: COMMA_AT -> type(COMMA_AT);
    STMT_COMMA: COMMA -> type(COMMA);
    STMT_HASH_SEMICOLON: HASH_SEMICOLON -> type(HASH_SEMICOLON);
    STMT_SEMICOLON: SEMICOLON -> type(SEMICOLON);
    
    STMT_POS_INFINITY: POS_INFINITY -> type(POS_INFINITY);
    STMT_NEG_INFINITY: NEG_INFINITY -> type(NEG_INFINITY);
    STMT_NAN: NAN -> type(NAN);
    STMT_BOOLEAN: BOOLEAN -> type(BOOLEAN);
    STMT_DOT_LITERAL: DOT_LITERAL -> type(DOT_LITERAL);
    STMT_ELLIPSIS_LITERAL: ELLIPSIS_LITERAL -> type(ELLIPSIS_LITERAL);
    STMT_UNDERSCORE_LITERAL: UNDERSCORE_LITERAL -> type(UNDERSCORE_LITERAL);
    STMT_BYTEVECTOR_PREFIX: BYTEVECTOR_PREFIX -> type(BYTEVECTOR_PREFIX);
    STMT_VECTOR_PREFIX: VECTOR_PREFIX -> type(VECTOR_PREFIX);
    STMT_HEX_PREFIXED_NUMBER: HEX_PREFIXED_NUMBER -> type(HEX_PREFIXED_NUMBER);
    STMT_OCTAL_PREFIXED_NUMBER: OCTAL_PREFIXED_NUMBER -> type(OCTAL_PREFIXED_NUMBER);
    STMT_BINARY_PREFIXED_NUMBER: BINARY_PREFIXED_NUMBER -> type(BINARY_PREFIXED_NUMBER);
    STMT_INTEGER: INTEGER -> type(INTEGER);
    STMT_COMPLEX: COMPLEX -> type(COMPLEX);
    STMT_FLOAT: FLOAT -> type(FLOAT);
    STMT_RATIO: RATIO -> type(RATIO);
    STMT_CHARACTER: CHARACTER -> type(CHARACTER);
    STMT_REGEX_PATTERN: REGEX_PATTERN -> type(REGEX_PATTERN);
    STMT_ESCAPED_IDENTIFIER: ESCAPED_IDENTIFIER -> type(ESCAPED_IDENTIFIER);
    STMT_IDENTIFIER: IDENTIFIER -> type(IDENTIFIER);
    STMT_STRING: STRING -> type(STRING);
    STMT_DECIMAL_PREFIX: DECIMAL_PREFIX -> type(DECIMAL_PREFIX);
    STMT_BLOCK_COMMENT: '/*' .*? '*/' -> type(BLOCK_COMMENT);
    STMT_LINE_COMMENT: '//' ~[\r\n]* -> type(LINE_COMMENT);