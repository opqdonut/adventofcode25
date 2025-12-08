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

: mem-swap {: a1 a2 cnt -- :}
    \ TODO: assert that pad is large enough!
    a1 pad cnt move
    a2 a1 cnt move
    pad a2 cnt move
;

: heap-swap {: addr i1 i2 -- :}
    ." SWAP(" i1 . i2 . ." )"
    addr i1 heap-th
    addr i2 heap-th
    heap-elem-size
    mem-swap
;

: heap-insert {: key addr | i -- :} \ TODO: return payload-addr
    ." INSERT(" key . ." )"
    addr addr heap-size heap-th ( addr-of-new-elem )
    dup swap !
    addr heap-size to i
    1 addr +! \ inc heap size
    key addr i heap-th !
    begin
        i 1 >=
        addr i heap-parent heap-th @
        key >
        and
    while
            addr i i heap-parent heap-swap
            i heap-parent to i
    repeat
;

: init-heap ( addr -- )
    0 swap !
;

: heap-peek-i ( i addr -- )
    swap heap-th @
;

: heap-top ( addr )
    0 heap-th @
;

: heap-pop {: addr | key i -- :}
    ." POP(" addr heap-top . ." )"
    \ bring last elem to root
    addr 0 addr heap-size 1- heap-swap
    -1 addr +! \ dec heap size
    0 to i
    addr 0 heap-th @ to key
    ." SINK(" key . ." )"
    begin
        \ TODO: nested ifs because there's no break/continue
        \ luckily we can use exit for break since this is the end of the function
        i heap-left addr heap-size >= if exit then
        addr i heap-left heap-th @
        key <
        if
            addr i i heap-left heap-swap
            i heap-left to i
        else
            i heap-right addr heap-size >= if exit then
            addr i heap-right heap-th @
            key <
            if
                addr i i heap-right heap-swap
                i heap-right to i
            else
                exit
            then
        then
    again
;

: .heap {: addr :}
    cr
    ." ["
    addr heap-size 0 +do
        i addr heap-peek-i .
    loop
    ." ]"
    cr
;

Create test-heap 256 cells allot

: test-heap-ops
    assert( heap-elem-size 24 = )
    test-heap init-heap
    assert( test-heap heap-size 0 = )
    13 test-heap heap-insert
    assert( test-heap heap-size 1 = )
    assert( test-heap heap-top 13 = )
    15 test-heap heap-insert
    assert( test-heap heap-size 2 = )
    assert( 0 test-heap heap-peek-i 13 = )
    assert( 1 test-heap heap-peek-i 15 = )
    16 test-heap heap-insert
    assert( test-heap heap-size 3 = )
    assert( 0 test-heap heap-peek-i 13 = )
    assert( 1 test-heap heap-peek-i 15 = )
    assert( 2 test-heap heap-peek-i 16 = )
    10 test-heap heap-insert
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
    8 test-heap heap-insert
    12 test-heap heap-insert
    9 test-heap heap-insert
    7 test-heap heap-insert
    3 test-heap heap-insert
    13 test-heap heap-insert
    10 test-heap heap-insert
    2 test-heap heap-insert
    1 test-heap heap-insert
    11 test-heap heap-insert
    4 test-heap heap-insert
    6 test-heap heap-insert
    5 test-heap heap-insert
    test-heap .heap
    assert( test-heap heap-size 13 = )
    assert( test-heap heap-top 1 = )
    test-heap heap-pop
    assert( test-heap heap-top 2 = )
    test-heap heap-pop
    assert( test-heap heap-top 3 = )
    test-heap heap-pop
    test-heap .heap
    assert( test-heap heap-top 4 ~~ = )
    test-heap heap-pop
    assert( test-heap heap-top 5 = )
    test-heap heap-pop assert( test-heap heap-size 9 = )
    assert( test-heap heap-top 6 = )
    test-heap heap-pop
    assert( test-heap heap-top 7 = )
    test-heap heap-pop
    assert( test-heap heap-top 8 = )
    test-heap heap-pop
    assert( test-heap heap-top 9 = )
    test-heap heap-pop
    assert( test-heap heap-top 10 = )
    test-heap heap-pop
    assert( test-heap heap-top 11 = )
    test-heap heap-pop
    assert( test-heap heap-top 12 = )
    test-heap heap-pop
    assert( test-heap heap-top 13 = )
    test-heap heap-pop
    assert( test-heap heap-size 0 = )

;

: tests
    test-heap-indexing
    test-heap-ops
    test-heap-ops-2
;