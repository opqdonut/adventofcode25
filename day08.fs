Create data 3000 cells allot
0 Value n-points

: skip-char ( ccc len -- ccc+1 len-1 )
    1- swap 1+ swap
;

: parse-number ( ccc len -- ccc len u )
    0 0 2swap ( ud ccc len )
    >number ( ud ccc len )
    2swap ( ccc len ud )
    drop
;

: parse-input ( ptr len -- )
    slurp-file
    0 to n-points
    begin
        parse-number
        data n-points th!
        n-points 1+ to n-points
        skip-char
        dup 0>
    while
    repeat
    assert( n-points 3 mod 0= )
    n-points 3 / to n-points
    2drop
;

: get-coord ( axis i -- )
    3 * + data swap th@
;

: show-point ( i -- )
    dup dup
    0 swap get-coord .
    1 swap get-coord .
    2 swap get-coord .
;

: show ( -- )
    cr
    n-points 0 +do
        i show-point
        cr
    loop
;

: dist {: i j -- u :}
    0 i get-coord 0 j get-coord - dup *
    1 i get-coord 1 j get-coord - dup * +
    2 i get-coord 2 j get-coord - dup * +
;

s" heap.fs" included

3000 3000 * heap-allot Value dist-heap

: dist-heap-top ( 0-or-1 -- i )
    cells dist-heap heap-top-payload + @
;

3000 cells allocate drop Value components
3000 cells allocate drop Value component-counts

: init-components ( -- )
    n-points 0 ?do
        i components i th!
    loop
;

: 2sort ( a b -- smaller larger )
    2dup > if swap then
;

: test-2sort
    assert( 1 2 2sort 2 = swap 1 = and )
    assert( 2 1 2sort 2 = swap 1 = and )
;

: merge ( a b -- )
    components swap th@
    swap components swap th@ ( a-comp b-comp )
    2sort ( min-comp max-comp )
    ." MERGE-COMPONENTS( " 2dup swap . . ." )"
    n-points 0 ?do
        2dup components i th@ = if
            components i th!
        else
            drop
        then
    loop
    2drop
;

: test-components
    6 to n-points
    init-components
    0 1 merge
    3 2 merge
    assert( components 0 th@ 0 = )
    assert( components 1 th@ 0 = )
    assert( components 2 th@ 2 = )
    assert( components 3 th@ 2 = )
    assert( components 4 th@ 4 = )
    assert( components 5 th@ 5 = )
    1 3 merge
    assert( components 0 th@ 0 = )
    assert( components 1 th@ 0 = )
    assert( components 2 th@ 0 = )
    assert( components 3 th@ 0 = )
    assert( components 4 th@ 4 = )
    assert( components 5 th@ 5 = )
;

: print-arr ( arr n -- )
    cr
    0 +do
        dup i th@ .
    loop
    cr
;

0 Value pairs-to-consider

: insert-pairs
    ." N " n-points . cr
    dist-heap init-heap
    n-points 0 ?do
        n-points i 1+ ?do
            \ ." PUSH( " j . i . j i dist . ." )" cr
            j i dist dist-heap heap-insert ( payload-addr )
            dup j swap 0 th!
            i swap 1 th!
        loop
    loop
    cr
    ." SMALLEST(" dist-heap heap-top . 0 dist-heap-top . 1 dist-heap-top . ." )"
    \ dist-heap .heap-full
;

: merge-one
    cr ." MERGE(" 0 dist-heap-top show-point ." + " 1 dist-heap-top show-point ." )"
    0 dist-heap-top 1 dist-heap-top merge
    dist-heap heap-pop
;

: solve1 ( -- u )
    insert-pairs

    init-components
    pairs-to-consider 0 +do
        merge-one
    loop

    components n-points print-arr

    component-counts n-points cells 0 fill
    n-points 0 +do
        1 component-counts components i th@ th +!
    loop

    component-counts n-points print-arr
    component-counts n-points heap-sort
    component-counts n-points print-arr

    1
    n-points n-points 3 - +do
        component-counts i th@ *
    loop
;

: example1
    s" day08.example" parse-input
    10 to pairs-to-consider
    solve1
    cr cr ." example1 " . cr
;

: part1
    s" day08.input" parse-input
    1000 to pairs-to-consider
    solve1
    cr cr ." example1 " . cr
;
\ answer 164475

: all-connected?
    n-points 0 +do
        components i th@
        0<> if false unloop exit then
    loop
    true
;

: solve2
    insert-pairs
    init-components
    0
    begin
        all-connected? invert while
            drop
            0 0 dist-heap-top get-coord
            0 1 dist-heap-top get-coord
            *
            merge-one
    repeat
;

: example2
    s" day08.example" parse-input
    solve2
    cr cr ." example2 " . cr
;

: part2
    s" day08.input" parse-input
    solve2
    cr cr ." part2 " . cr
;
\ answer 169521198

: tests
    test-2sort
    test-components
;