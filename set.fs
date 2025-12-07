\ ordered sets of cells, represented as sorted arrays

: set-find-index {: thresh addr n -- i :}
    \ cr ." FIND(" thresh . ." )"
    n 0 +do
        addr i th@ ( cur )
        \ ." LOOK(" i . dup . ." )"
        thresh >= if i unloop exit then
    loop
    n
;

: set-lookup {: thresh addr n -- i exact? :}
    thresh addr n set-find-index
    addr over th@ thresh =
    over n <
    and
;

: set-contains ( elem addr n -- found? )
    set-lookup nip
;

: set-insert {: new addr n | i -- addr n :}
    new addr n set-lookup ( i exact? )
    if drop addr n exit then
    to i
    \ ." insert at " i .
    addr i th addr i 1+ th ( iptr iptr+1 )
    n i - cells ( iptr iptr+1 count )
    move
    new addr i th!
    addr n 1+
;

: set-remove {: elem addr n | i -- addr n :}
    elem addr n set-lookup
    invert if drop addr n exit then
    to i
    \ ." remove from " i .
    addr i 1+ th addr i th
    n i - cells
    move
    addr n 1-
;

: .set ( addr n )
    ." SET("
    0 +do
        dup i th@ .
    loop
    drop
    ." )"
;

Create test-set 265 cells allot

: tests {: | n :}
    0 to n

    3 test-set n set-insert
    to n
    assert( test-set = )
    assert( n 1 = )
    assert( test-set @ 3 = )
    assert( 3 test-set n set-find-index 0 = )
    assert( 4 test-set n set-find-index 1 = )

    4 test-set n set-insert
    to n
    assert( test-set = )
    assert( n 2 = )
    assert( test-set 0 th@ 3 = )
    assert( test-set 1 th@ 4 = )
    assert( 3 test-set n set-find-index 0 = )
    assert( 4 test-set n set-find-index 1 = )
    assert( 6 test-set n set-find-index 2 = )

    1 test-set n set-insert
    to n
    assert( test-set = )
    assert( n 3 = )
    assert( test-set 0 th@ 1 = )
    assert( test-set 1 th@ 3 = )
    assert( test-set 2 th@ 4 = )
    assert( 1 test-set n set-find-index 0 = )
    assert( 2 test-set n set-find-index 1 = )
    assert( 3 test-set n set-find-index 1 = )
    assert( 4 test-set n set-find-index 2 = )
    assert( 6 test-set n set-find-index 3 = )
    assert( 0 test-set n set-contains invert )
    assert( 1 test-set n set-contains )
    assert( 2 test-set n set-contains invert )
    assert( 3 test-set n set-contains )
    assert( 4 test-set n set-contains )
    assert( 5 test-set n set-contains invert )

    2 test-set n set-remove
    to n
    assert( test-set = )
    test-set n .set
    assert( n 3 = )

    3 test-set n set-remove
    to n
    assert( test-set = )
    test-set n .set
    assert( n 2 = )
    assert( test-set 0 th@ 1 = )
    assert( test-set 1 th@ 4 = )
    assert( 3 test-set n set-contains invert )

;