S' : S ;
S : S '+' T | T ;
T : T '*' F | F ;
F : '(' S ')' | 'num' ;