0 Value infile
Create line-buffer 256 allot

0 Value n-ranges
Create los 256 cells allot
Create his 256 cells allot
0 Value n-input
Create input 1024 cells allot

: parse-number ( ccc len -- ccc len u )
    0 0 2swap ( ud ccc len )
    >number ( ud ccc len )
    2swap ( ccc len ud )
    drop
;

: skip-char ( ccc len -- ccc+1 len-1 )
    1- swap 1+ swap
;

: read-input ( ccc len -- )
    r/o open-file throw to infile
    0 to n-ranges
    begin
        line-buffer line-buffer 256 infile read-line throw
        ( pointer len end? )
        over 0> and
    while
            parse-number los n-ranges th!
            skip-char parse-number his n-ranges th!
            2drop
            n-ranges 1+ to n-ranges
    repeat
    2drop

    0 to n-input
    begin
        line-buffer line-buffer 256 infile read-line throw
    while
            parse-number input n-input th!
            2drop
            n-input 1+ to n-input
    repeat
    2drop
;

: show ( -- )
    cr
    n-ranges 0 ?do
        los i th@ .
        ." -"
        his i th@ .
        cr
    loop
    n-input 0 ?do
        input i th@ .
        cr
    loop
;

: in-range? ( n lo hi -- f )
    third >= -rot >= and
;

: fresh? ( n -- )
    n-ranges 0 ?do
        dup
        los i th@
        his i th@
        in-range? if unloop drop true exit then
    loop
    drop
    false
;

: solve1 ( -- )
    0
    n-input 0 ?do
        input i th@ fresh? if 1 + then
    loop
;

: example1 ( -- )
    s" day05.example" read-input
    solve1
    cr cr ." example1 " . cr
;

: part1 ( -- )
    s" day05.input" read-input
    solve1
    cr cr ." part1 " . cr
;
\ answer 511
