0 Value infile
Create line-buffer 4096 allot

0 Value n-lines
0 Value line-len
0 Value write-to
Create data 10000 cells allot

: skip-char ( ccc len -- ccc+1 len-1 )
    1- swap 1+ swap
;

: parse-number ( ccc len -- ccc len u )
    over c@
    case
        '+' of skip-char -1 exit endof
        '*' of skip-char -2 exit endof
    endcase
    0 0 2swap ( ud ccc len )
    >number ( ud ccc len )
    2swap ( ccc len ud )
    drop
    dup assert( 0> )
;

: test-parse-number
    assert( s" +" parse-number -1 = 2drop )
    assert( s" *" parse-number -2 = 2drop )
    assert( s" 123" parse-number 123 = 2drop )

;

: skip-whitespace ( ccc len -- ccc len )
    begin
        over c@ 32 = \ 32 is ' '
    while
            skip-char
    repeat
;

: parse-input ( ccc len -- )
    r/o open-file throw to infile
    0 to write-to
    0 to n-lines
    0 to line-len
    begin
        line-buffer line-buffer 4096 infile read-line throw
        ( pointer len end? )
    while
            begin
                skip-whitespace
                parse-number data write-to th!
                ." WRITE(" n-lines . line-len . write-to . data write-to th@ . ." )"
                write-to 1+ to write-to
                n-lines 0= if line-len 1+ to line-len then
                skip-whitespace
                dup 0> while
            repeat
            n-lines 1+ to n-lines
            2drop
    repeat
    2drop
;

: test-parse-input
    s" day06.example" parse-input
    assert( n-lines 4 = )
    assert( line-len 4 = )
    assert( data 0 th@ 123 .s = )
    assert( data 1 th@ 328 = )
    assert( data 5 th@ 64 = )
    assert( data 12 th@ -2 = )
    assert( data 15 th@ -1 = )
;

: tests
    test-parse-number
    test-parse-input
;

: 2th@ ( line pos -- u )
    swap line-len * + data swap th@
;

: operate ( u u pos -- u )
    n-lines 1- swap 2th@ case
        -1 of ." +" + endof
        -2 of ." *" * endof
        assert( false )
    endcase
;

: solve1
    0
    line-len 0 +do
        ." column " i . cr
        0 i 2th@
        dup .
        n-lines 1- 1 +do
            i j 2th@
            dup .
            j operate
            cr
        loop
        +
    loop
;

: example1
    s" day06.example" parse-input
    solve1
    cr cr ." example1 " . cr
;

: part1
    s" day06.input" parse-input
    solve1
    cr cr ." part1 " . cr
;
\ answer 7098065460541