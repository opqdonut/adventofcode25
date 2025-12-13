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

: add-neighbour ( neighbour where-to-add )
    neighbours
    max-neighbours 0 +do
        dup i th@ -1 = if
            i th!
            unloop exit
        then
    loop
    assert( false )
;

: parse-input {: | from :}
    graph graph-size -1 fill
    r/o open-file throw to infile
    begin
        line-buffer line-buffer 512 infile read-line throw
    while
            ( pointer len )
            id2int to from
            skip-char skip-char \ ": "
            begin
                dup 0>
            while
                    \ build grap with reversed edges!
                    \ aaa: bbb ccc
                    \ means we add aaa to the neighbour-list of bbb and ccc
                    id2int from swap add-neighbour
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


0 Value count

: visit ( id -- )
    cr ." VISIT " dup .i
    dup out = if
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

s" you" id2int Value you 2drop
s" out" id2int Value out 2drop
s" svr" id2int Value svr 2drop
s" fft" id2int Value fft 2drop
s" dac" id2int Value dac 2drop

s" heap.fs" included

\ -1 means not visited, >=0 is valid path count
Create path-counts max-node cells allot

\ Create queue max-node cells allot
\ 0 Value queue-read
\ 0 Value queue-write

: visit-count-paths ( id -- )
    cr ." VISIT " dup .i
    \ path count done, no need to proceed
    path-counts over th@ 0>= if
        ." DONE "
        drop exit
    then
    \ traverse edges that point to us
    max-neighbours 0 +do
        dup neighbours i th@
        dup -1 = if
            drop leave
        else
            recurse
        then
    loop
    \ now our predecessors have path-counts, our path count is the sum
    ( id )
    cr ." SUM " dup .i ." FROM "
    0 path-counts third th! \ clear the -1 marker
    max-neighbours 0 +do
        dup neighbours i th@ ( id neigh )
        dup -1 = if
            drop leave
        else
            dup .i
            path-counts swap th@ ( id pred-count )
            path-counts third th +!
        then
    loop
    ." = " path-counts over th@ .
    drop
;

: count-paths ( end init -- )
    path-counts max-node cells -1 fill
    1 path-counts rot th!
    visit-count-paths
;

: solve1 {: | cur :}
    out you count-paths
    path-counts out th@
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

: solve2 {: | svr2dac svr2fft dac2fft fft2dac fft2out dac2out :}
    out svr count-paths
    path-counts dac th@ to svr2dac
    path-counts fft th@ to svr2fft
    out fft count-paths
    path-counts dac th@ to fft2dac
    path-counts out th@ to fft2out
    out dac count-paths
    path-counts fft th@ to dac2fft
    path-counts out th@ to dac2out

    svr2dac dac2fft * fft2out *
    svr2fft fft2dac * dac2out *
    +
;

: example2
    s" day11.example2" parse-input
    solve2
    cr cr ." example2 " . cr
;

: part2
    s" day11.input" parse-input
    solve2
    cr cr ." part2 " . cr
;
\ answer 520476725037672

: tests
    test-id2int
;