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

: swp {: arr i j | tmp -- :}
    arr i th@ to tmp
    arr j th@ arr i th!
    tmp arr j th!
;

: sort-ranges ( -- )
    \ I never thought I'd find myself implementing bubble sort
    begin
        false
        n-ranges 1 ?do
            ." LOOP " i . cr
            los i 1- th@
            los i th@
            > if
                ." SWAP " cr
                los i i 1- swp
                his i i 1- swp
                drop true
            then
        loop
    while
    repeat
;

: solve2 {: | res -- :}
    los 0 th@
    his 0 th@
    n-ranges 1 ?do
        ." STATE( " 2dup swap . . ." )" cr
        ." CONSIDER( " los i th@ . his i th@ . ." )" cr
        assert( over los i th@ <= )
        dup los i th@ >= if
            ." OVERLAP " cr
            \ ranges overlap:
            \ (lo,hi),(lo2,hi2) => (lo,max hi hi2)
            his i th@ max
        else
            ." INCREMENT "
            \ ranges disjoint:
            \ increment result, start processing from new range
            swap - 1+ dup . res + dup . to res
            los i th@
            his i th@
            cr
        then
    loop
    \ final increment
    ." STATE( " 2dup swap . . ." )" cr
    ." INCREMENT "
    swap - 1+ dup . res + dup .
;

: example2 ( -- )
    s" day05.example" read-input
    sort-ranges
    solve2
    cr cr ." example2 " . cr
;

: part2 ( -- )
    s" day05.input" read-input
    sort-ranges
    solve2
    cr cr ." part2 " . cr
;
\ answer 350939902751909