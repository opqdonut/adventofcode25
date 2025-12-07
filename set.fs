\ ordered sets of cells, represented as sorted arrays

: set-find-index {: addr n thresh -- i :}
    \ cr ." FIND(" thresh . ." )"
    n 0 +do
        addr i th@ ( cur )
        \ ." LOOK(" i . dup . ." )"
        thresh >= if i unloop exit then
    loop
    n
;

: set-lookup {: addr n thresh -- i exact? :}
    addr n thresh set-find-index
    addr over th@ thresh =
    over n <
    and
;

: set-contains ( addr n elem -- found? )
    set-lookup nip
;

: set-insert {: addr n new | i -- addr n :}
    addr n new set-lookup ( i exact? )
    if drop addr n exit then
    to i
    \ ." insert at " i .
    addr i th addr i 1+ th ( iptr iptr+1 )
    n i - cells ( iptr iptr+1 count )
    move
    new addr i th!
    addr n 1+
;

: set-remove {: addr n elem | i -- addr n :}
    addr n elem set-lookup
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

    test-set n 3 set-insert
    to n
    assert( test-set = )
    assert( n 1 = )
    assert( test-set @ 3 = )
    assert( test-set n 3 set-find-index 0 = )
    assert( test-set n 4 set-find-index 1 = )

    test-set n 4 set-insert
    to n
    assert( test-set = )
    assert( n 2 = )
    assert( test-set 0 th@ 3 = )
    assert( test-set 1 th@ 4 = )
    assert( test-set n 3 set-find-index 0 = )
    assert( test-set n 4 set-find-index 1 = )
    assert( test-set n 6 set-find-index 2 = )

    test-set n 1 set-insert
    to n
    assert( test-set = )
    assert( n 3 = )
    assert( test-set 0 th@ 1 = )
    assert( test-set 1 th@ 3 = )
    assert( test-set 2 th@ 4 = )
    assert( test-set n 1 set-find-index 0 = )
    assert( test-set n 2 set-find-index 1 = )
    assert( test-set n 3 set-find-index 1 = )
    assert( test-set n 4 set-find-index 2 = )
    assert( test-set n 6 set-find-index 3 = )
    assert( test-set n 0 set-contains invert )
    assert( test-set n 1 set-contains )
    assert( test-set n 2 set-contains invert )
    assert( test-set n 3 set-contains )
    assert( test-set n 4 set-contains )
    assert( test-set n 5 set-contains invert )

    test-set n 2 set-remove
    to n
    assert( test-set = )
    test-set n .set
    assert( n 3 = )

    test-set n 3 set-remove
    to n
    assert( test-set = )
    test-set n .set
    assert( n 2 = )
    assert( test-set 0 th@ 1 = )
    assert( test-set 1 th@ 4 = )
    assert( test-set n 3 set-contains invert )

;