0 Value infile

25000 Constant file-max-len
Create file-array file-max-len allot
0 Value file-len
0 Value file-line-len
0 Value file-n-lines
0 Value start

: parse-input
    r/o open-file throw to infile
    file-array file-max-len infile read-file
    assert( invert )
    to file-len
    assert( file-len file-max-len < )
    file-len 0 +do
        file-array i + c@
        case
            \ '\n'
            10 of i 1+ to file-line-len leave endof
            'S' of i to start endof
        endcase
    loop
    assert( file-len file-line-len mod 0= )
    file-len file-line-len / to file-n-lines
;

: 2dc! ( c col line -- )
    file-line-len * + file-array + c!
;

: 2dc@ ( col line -- c )
    file-line-len * + file-array + c@
;

Create beams 256 allot
0 Value n-beams

: beam@ ( i -- c )
    beams + c@
;

: find-beam ( thresh -- i )
    \ cr ." FIND(" dup . ." )"
    n-beams 0 +do
        dup i beam@ ( thres thresh cur )
        \ ." LOOK(" i . dup . ." )"
        <= if drop i unloop exit then
    loop
    drop n-beams
;

\ TODO: factor commonality with insert-beam and remove-beam
\ TODO: factor out into a library
: contains-beam ( beam -- )
    dup find-beam ( beam i )
    dup beam@ ( beam i found )
    rot = ( i matches )
    swap n-beams < ( matches i<len )
    and
;

: remove-beam ( beam -- )
    dup find-beam ( beam i -- )
    \ exists check
    dup beam@ ( beam i found )
    third = ( beam i matches )
    over n-beams < ( beam i matches i<len )
    and invert if
        \ ." not-present"
        2drop exit then
    \ ." remove"
    nip ( i )
    dup beams + 1 + over beams + ( i iptr+1 iptr )
    rot n-beams swap - ( iptr+1 iptr count )
    cmove
    n-beams 1- to n-beams
;

: insert-beam ( beam -- )
    dup find-beam ( beam i -- )
    \ exists
    dup beam@ ( beam i found )
    third = ( beam i matches )
    over n-beams < ( beam i matches i<len )
    and if
        \ ." exists"
        2drop exit then
    \ insert
    \ ." insert"
    dup dup beams + swap beams + 1+ ( beam i iptr iptr+1 )
    third n-beams swap - ( beam i iptr iptr+1 count )
    cmove> ( beam i )
    beams + c!
    n-beams 1+ to n-beams
;

: test-beams
    0 to n-beams
    3 insert-beam
    assert( n-beams 1 = )
    assert( beams c@ 3 = )
    assert( 3 find-beam 0 = )
    assert( 4 find-beam 1 = )
    4 insert-beam
    assert( n-beams 2 = )
    assert( beams c@ 3 = )
    assert( beams 1 + c@ 4 = )
    assert( 3 find-beam 0 = )
    assert( 4 find-beam 1 = )
    assert( 6 find-beam 2 = )
    1 insert-beam
    assert( n-beams 3 = )
    assert( beams c@ 1 = )
    assert( beams 1 + c@ 3 = )
    assert( beams 2 + c@ 4 = )
    assert( 1 find-beam 0 = )
    assert( 2 find-beam 1 = )
    assert( 3 find-beam 1 = )
    assert( 4 find-beam 2 = )
    assert( 6 find-beam 3 = )
    assert( 0 contains-beam invert )
    assert( 1 contains-beam )
    assert( 2 contains-beam invert )
    assert( 3 contains-beam )
    assert( 4 contains-beam )
    assert( 5 contains-beam invert )

    2 remove-beam
    assert( n-beams 3 = )
    3 remove-beam
    assert( n-beams 2 = )
    assert( beams c@ 1 = )
    assert( beams 1 + c@ 4 = )
    assert( 3 contains-beam invert )
;

: simulate ( -- )
    0 ( n-splits )
    0 to n-beams
    start insert-beam
    file-n-lines 1 +do
        file-line-len 0 +do
            i j 2dc@ '^' = if
                i contains-beam if
                    ." split!"
                    i remove-beam
                    i 1+ insert-beam
                    i 1- insert-beam
                    1+
                    dup . cr
                else
                    \ debug: mark non-splitting splitters
                    '#' i j 2dc!
                then
            then
        loop
        \ visualize
        file-line-len 0 +do
            i contains-beam
            if '|' else i j 2dc@ then emit
        loop
    loop
;

: example1
    s" day07.example" parse-input
    cr simulate
    cr cr ." example1 " . cr
;

: part1
    s" day07.input" parse-input
    cr simulate
    cr cr ." part1 " . cr
;
\ 1526 too low

: tests
    test-beams
;