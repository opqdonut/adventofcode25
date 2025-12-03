0 Value infile
Create line-buffer 256 allot

: getline ( -- ccc len end? )
    line-buffer
    line-buffer 256 infile read-line throw
;

: str-max ( ccc len start -- char )
    -1 -rot '.' -rot
    ?do
        ( ccc maxi maxc )
        third i + c@
        ( ccc maxi maxc c' )
        over over ( ccc maxi maxc c' max c' )
        < if -rot drop drop i swap else drop then
    loop
    ( ccc maxi maxc )
    rot drop
;

: eq2 ( x1 y1 x2 y2 -- f )
    rot ( x1 x2 y2 y1 )
    = ( x1 x2 yf )
    -rot ( yf x1 x2 )
    = ( yf xf )
    and
;

: str-max-t
    assert( s" 01234321" 0 str-max 4 '4' eq2 )
    assert( s" 01234321" 5 str-max 5 '3' eq2 )
    assert( s" 1119171" 0 str-max 3 '9' eq2 )
    assert( s" 1119171" 4 str-max 5 '7' eq2 )
    assert( s" 12346" 0 str-max 4 '6' eq2 )
    assert( s" 12346" 3 str-max 4 '6' eq2 )
;

: digit-to-int ( c -- c )
    '0' -
;

: -rot4
    3 roll 3 roll 3 roll
;

: max-joltage ( ccc len -- n )
    \ first digit picked must not be last digit of string
    over over 1- 0 ( ccc len ccc len-1 0 )
    str-max ( ccc len maxi maxc )
    \ second digit comes after first digit
    -rot4 ( maxc ccc len maxi )
    1+ str-max ( maxc maxi' maxc' )
    swap drop ( maxc maxc' )
    digit-to-int swap digit-to-int 10 * +
;

: max-joltage-t
    assert( s" 987654321111111" max-joltage 98 = )
    assert( s" 811111111111119" max-joltage .s 89 = )
;

: part1
    s" day03.input" r/o open-file throw to infile
    0
    begin
        getline
    while
            max-joltage
            +
    repeat
    drop drop

    cr cr ." part1 " .
;
\ answer 17074

: tests
    str-max-t
    max-joltage-t
;