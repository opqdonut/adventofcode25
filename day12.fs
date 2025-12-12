0 Value infile
Create line-buffer 100 allot
s" utils.fs" included

\ let's see if it's enough to sum the sizes of the blocks
Create sizes 7 , 7 , 6 , 7 , 5 , 7 ,
6 Value n-pieces

: part1 {: | field-size total-size :}
    s" day12.input" r/o open-file throw to infile
    \ skip pieces
    30 0 +do
        line-buffer 100 infile read-line throw drop drop
    loop
    0
    begin
        line-buffer line-buffer 512 infile read-line throw
    while
            ( acc pointer len )
            parse-number to field-size
            skip-char \ "x"
            parse-number field-size * to field-size
            skip-char \ ":"
            0 to total-size
            n-pieces 0 do
                skip-char \ " "
                parse-number sizes i th@ *
                total-size + to total-size
            loop
            2drop
            cr ." FIELD " field-size . ." TOTAL " total-size .
            total-size field-size <= if
                1+
            then
    repeat
    2drop
    cr ." part1 " . cr
;
\ answer 505