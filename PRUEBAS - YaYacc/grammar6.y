S' : E ;
E : E 'or' T | T ;
T : T 'and' F | F ;
F : 'not' F | '(' E ')' ;
F : 'true' | 'false' ;