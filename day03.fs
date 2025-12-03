0 Value infile
Create line-buffer 256 allot

: getline ( -- ccc len end? )
    line-buffer
    line-buffer 256 infile read-line throw
;

: max-char ( end start -- pointer )
    dup 1- '.' 2swap ( initiali initialc end start )
    ?do
        dup i c@ ( maxi maxc maxc c )
        < if 2drop i i c@ then
    loop
    drop
;

: max-char-t
    assert( s" 012345" line-buffer swap cmove
    line-buffer 6 + line-buffer max-char c@ '5' = )
;

: digit-to-int ( c -- c )
    '0' -
;

: max-joltage ( ccc len -- n )
    over + swap ( end start )
    \ first digit must not be last digit of string
    over 1- swap ( end end-1 start )
    max-char ( end max-pointer )
    dup c@ digit-to-int -rot ( digit1 end max-pointer )
    \ second digit comes after first digit
    1+ max-char ( digit1 max-pointer )
    c@ digit-to-int ( digit1 digit 2 )
    swap 10 * +
;

: max-joltage-t
    assert( s" 987654321111111" max-joltage .s 98 = )
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

0 Value current

: add-digit ( c -- )
    c@ digit-to-int current 10 * + to current
;

: max-joltage-2 {: start n | end -- n :}
    0 to current
    start n + to end

    end 11 - start max-char ( max-pointer )
    dup add-digit ( max-pointer )

    end 10 - swap 1+ max-char dup add-digit
    end 9 - swap 1+ max-char dup add-digit
    end 8 - swap 1+ max-char dup add-digit
    end 7 - swap 1+ max-char dup add-digit
    end 6 - swap 1+ max-char dup add-digit
    end 5 - swap 1+ max-char dup add-digit
    end 4 - swap 1+ max-char dup add-digit
    end 3 - swap 1+ max-char dup add-digit
    end 2 - swap 1+ max-char dup add-digit
    end 1 - swap 1+ max-char dup add-digit
    end 0 - swap 1+ max-char dup add-digit
    drop current
;

: max-joltage-2-t
    assert( s" 987654321111111" max-joltage-2 987654321111 = )
    assert( s" 811111111111119" max-joltage-2 811111111119 = )
    assert( s" 234234234234278" max-joltage-2 434234234278 = )
;

: part2
    s" day03.input" r/o open-file throw to infile
    0
    begin
        getline
    while
            max-joltage-2
            +
    repeat
    drop drop

    cr cr ." part2 " .
;
\ answer 169512729575727

: tests
    max-char-t
    max-joltage-t
    max-joltage-2-t
;