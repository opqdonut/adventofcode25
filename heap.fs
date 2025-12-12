\ binary (min)heaps

\ memory layout
\ [siz] [key0] [payload0 ...] [key1] [payload0 ...]
\ 0     8      16

\ how much data to store in each heap-entry
2 cells Value heap-payload-len

: heap-elem-size ( -- u )
    cell heap-payload-len +
;

: heap-left 2 * 1 + ;
: heap-right 2 * 2 + ;
: heap-parent 1- 2 / ;

: test-heap-indexing
    assert( 0 heap-left 1 = )
    assert( 0 heap-right 2 = )
    assert( 1 heap-parent 0 = )
    assert( 2 heap-parent 0 = )

    assert( 1 heap-left 3 = )
    assert( 1 heap-right 4 = )
    assert( 2 heap-left 5 = )
    assert( 2 heap-right 6 = )
    assert( 3 heap-parent 1 = )
    assert( 4 heap-parent 1 = )
    assert( 5 heap-parent 2 = )
    assert( 6 heap-parent 2 = )
;

: heap-size @ ;

: heap-th ( addr i -- addr )
    heap-elem-size * cell + +
;

: heap-peek-i ( i addr -- )
    swap heap-th @
;

: mem-swap {: a1 a2 cnt -- :}
    \ TODO: assert that pad is large enough!
    a1 pad cnt move
    a2 a1 cnt move
    pad a2 cnt move
;

: heap-swap {: i1 i2 addr -- :}
    \ ." SWAP(" i1 . i2 . ." )"
    addr i1 heap-th
    addr i2 heap-th
    heap-elem-size
    mem-swap
;

: heap-insert {: key addr | i -- payload-addr :}
    \ ." INSERT(" key . ." )"
    addr addr heap-size heap-th ( addr-of-new-elem )
    dup swap !
    addr heap-size to i
    1 addr +! \ inc heap size
    key addr i heap-th !
    begin
        i 1 >=
        if i heap-parent addr heap-peek-i key >
        else false
        then
    while
            i i heap-parent addr heap-swap
            i heap-parent to i
    repeat
    addr i heap-th cell +
;

: init-heap ( addr -- )
    0 swap !
;

: heap-allot ( n -- addr )
    heap-elem-size * allocate assert( invert )
    dup init-heap
;

: heap-top ( addr )
    0 heap-th @
;

: heap-top-payload ( addr )
    0 heap-th cell +
;

: heap-pop {: addr | key i newi -- :}
    \ cr ." POP(" addr heap-top . ." )"
    \ bring last elem to root
    0 addr heap-size 1- addr heap-swap
    -1 addr +! \ dec heap size
    0 to i
    addr 0 heap-th @ to key
    \ ." SINK(" key . ." )"
    begin
        \ find lower of existing children
        i heap-left addr heap-size >= if exit then \ no children: we're done
        \ assume we'll swap with left child
        i heap-left to newi
        \ if right child exists and is smaller, swap with it instead
        i heap-right addr heap-size < if
            newi addr heap-peek-i
            i heap-right addr heap-peek-i
            > if
                i heap-right to newi
            then
        then
        \ if candidate child is larger, we're done
        newi addr heap-peek-i key >= if exit then

        i newi addr heap-swap
        newi to i
    again
;

: .heap {: addr :}
    cr
    ." ["
    0 addr heap-peek-i .
    addr heap-size 1 +do
        i addr heap-peek-i .
        i addr heap-peek-i
        i heap-parent addr heap-peek-i
        < if
            ." <-!" i heap-parent addr heap-peek-i . ." !> "
        then
    loop
    ." ]"
    cr
;

: .heap-full {: addr :}
    cr
    ." [" cr
    addr heap-size 0 +do
        ."  "
        i addr heap-peek-i .
        i 0< if
            i addr heap-peek-i
            i heap-parent addr heap-peek-i
            < if
                ." <-! "
            then
        then
        ." ("
        addr i heap-th cell + dup heap-payload-len + swap +do
            i @ .
            cell +loop
        ." ) " cr
    loop
    ." ]"
    cr
;

256 heap-allot Value test-heap
\ Create test-heap 256 cells allot

: test-heap-ops
    assert( heap-elem-size 24 = )
    test-heap init-heap
    assert( test-heap heap-size 0 = )
    13 test-heap heap-insert drop
    assert( test-heap heap-size 1 = )
    assert( test-heap heap-top 13 = )
    15 test-heap heap-insert drop
    assert( test-heap heap-size 2 = )
    assert( 0 test-heap heap-peek-i 13 = )
    assert( 1 test-heap heap-peek-i 15 = )
    16 test-heap heap-insert drop
    assert( test-heap heap-size 3 = )
    assert( 0 test-heap heap-peek-i 13 = )
    assert( 1 test-heap heap-peek-i 15 = )
    assert( 2 test-heap heap-peek-i 16 = )
    10 test-heap heap-insert drop
    assert( test-heap heap-size 4 = )
    assert( 0 test-heap heap-peek-i 10 = )
    assert( 1 test-heap heap-peek-i 13 = )
    assert( 2 test-heap heap-peek-i 16 = )
    assert( 3 test-heap heap-peek-i 15 = )
    test-heap heap-pop
    assert( test-heap heap-size 3 = )
    assert( 0 test-heap heap-peek-i 13 = )
    assert( 1 test-heap heap-peek-i 15 = )
    assert( 2 test-heap heap-peek-i 16 = )
;

: test-heap-ops-2
    assert( heap-elem-size 24 = )
    test-heap init-heap
    assert( test-heap heap-size 0 = )
    8 test-heap heap-insert drop
    12 test-heap heap-insert drop

    9 test-heap heap-insert
    99999 swap !

    7 test-heap heap-insert drop
    3 test-heap heap-insert drop
    13 test-heap heap-insert drop
    10 test-heap heap-insert drop
    2 test-heap heap-insert drop
    1 test-heap heap-insert drop
    11 test-heap heap-insert drop
    4 test-heap heap-insert drop
    6 test-heap heap-insert drop
    5 test-heap heap-insert drop

    test-heap .heap-full

    test-heap .heap
    assert( test-heap heap-size 13 = )
    assert( test-heap heap-top 1 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 2 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 3 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 4 = )
    test-heap heap-pop
    assert( test-heap heap-top 5 = )
    test-heap heap-pop assert( test-heap heap-size 8 = )
    assert( test-heap heap-top 6 = )
    test-heap heap-pop
    assert( test-heap heap-top 7 = )
    test-heap heap-pop
    assert( test-heap heap-top 8 = )
    test-heap heap-pop
    assert( test-heap heap-top 9 = )
    assert( test-heap heap-top-payload @ 99999 = )

    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 10 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 11 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 12 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 13 = )
    test-heap heap-pop
    assert( test-heap heap-size 0 = )

;

: heap-sort {: arr n | heap -- }
    \ TODO: not in-place!
    n heap-allot to heap
    n 0 +do
        \ cr ." IN " arr i th@ .
        arr i th@ heap heap-insert drop
        \ heap .heap
    loop
    n 0 +do
        \ cr ." OUT " heap heap-top .
        heap heap-top arr i th!
        heap heap-pop
        \ heap .heap
    loop
    \ cr
;

12 Value test-sort-n
Create test-sort-in 29 , 79 , 28 , 72 , 92 , 12 , 37 , 23 , 85 , 6 , 14 , 79 ,
Create test-sort-exp 6 , 12 , 14 , 23 , 28 , 29 , 37 , 72 , 79 , 79 , 85 , 92 ,

: test-heap-sort
    test-sort-in test-sort-n heap-sort
    test-sort-n 0 +do
        ." CHECK(" i . test-sort-in i th . ." )"
        assert( test-sort-in i th@ test-sort-exp i th@ = )
    loop
;

: tests
    test-heap-indexing
    test-heap-ops
    test-heap-ops-2
    test-heap-sort
;