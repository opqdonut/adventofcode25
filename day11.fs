s" utils.fs" included

: id2int ( ccc len -- ccc len d )
    over c@ 'a' - 26 * 26 *
    third 1 + c@ 'a' - 26 * +
    third 2 + c@ 'a' - +
    -rot skip-char skip-char skip-char rot
;

: .i ( d -- )
    dup 26 26 * / 'a' + emit
    26 26 * mod
    dup 26 / 'a' + emit
    26 mod
    'a' + emit
    32 emit
;

: test-id2int
    assert( s" aaa" id2int 0 = 2drop )
    assert( s" aab" id2int 1 = 2drop )
    assert( s" aaz" id2int 25 = 2drop )
    assert( s" aba" id2int 26 = 2drop )
    assert( s" abz" id2int 26 25 + = 2drop )
    assert( s" aca" id2int 26 2 * = 2drop )
    assert( s" azz" id2int 26 25 * 25 + = 2drop )
    assert( s" baa" id2int 26 26 * = 2drop )
;
s" zzz" id2int 1+ nip nip Value max-node
30 Value max-neighbours
max-node max-neighbours * cells Value graph-size
Create graph  graph-size allot
0 Value infile
Create line-buffer 512 allot

: neighbours ( id -- addr )
    graph swap max-neighbours * th
;

: parse-input {: | from to-count :}
    graph graph-size -1 fill
    r/o open-file throw to infile
    begin
        line-buffer line-buffer 512 infile read-line throw
    while
            ( pointer len )
            id2int to from
            0 to to-count
            skip-char skip-char \ ": "
            begin
                dup 0>
            while
                    id2int from neighbours to-count th!
                    to-count 1+ to to-count
                    skip-char \ " "
            repeat
            2drop
    repeat
    2drop
;

: show
    max-node 0 +do
        i neighbours 0 th@ -1 <> if
            cr i .i ." : "
            max-neighbours 0 +do
                j neighbours i th@
                dup -1 = if drop leave then
                .i
            loop
        then
    loop
;

s" you" id2int Value start 2drop
s" out" id2int Value stop 2drop
0 Value count

: visit ( id -- )
    cr ." VISIT " dup .i
    dup stop = if
        count 1+ to count drop exit
    then
    neighbours
    max-neighbours 0 +do
        dup i th@
        dup -1 = if
            drop leave
        else
            recurse
        then
    loop
    drop
;

: solve1 {: | cur :}
    start visit
    count
;

: example1
    s" day11.example" parse-input
    solve1
    cr cr ." example1 " . cr
;

: part1
    s" day11.input" parse-input
    solve1
    cr cr ." part1 " . cr
;
\ answer 791

: tests
    test-id2int
;