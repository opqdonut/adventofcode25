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

s" set.fs" included

Create beams 256 cells allot
0 Value n-beams

: contains-beam ( beam -- f )
    beams n-beams set-contains
;

: remove-beam ( beam -- )
    beams n-beams set-remove
    to n-beams
    drop
;

: insert-beam ( beam -- )
    beams n-beams set-insert
    to n-beams
    drop
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
\ answer 1573

Create timelines 256 cells allot

: th+! ( n arr i -- )
    cells + +!
;

: simulate2
    timelines 256 cells 0 fill
    1 timelines start th!
    file-n-lines 1 +do
        file-line-len 0 +do
            i j 2dc@ '^' = if
                timelines i th@ dup
                timelines i 1+ th+!
                timelines i 1- th+!
                0 timelines i th!
            then
        loop
        \ debug
        cr file-line-len 0 +do
            ."    " i j 2dc@ emit
        loop
        cr file-line-len 0 +do
            timelines i th@ 4 .r
        loop
    loop
    \ sum
    0
    file-line-len 0 +do
        timelines i th@
        +
    loop
;

: example2
    s" day07.example" parse-input
    simulate2
    cr cr ." example2 " . cr
;

: part2
    s" day07.input" parse-input
    simulate2
    cr cr ." part2 " . cr
;
\ answer 15093663987272

: tests
;