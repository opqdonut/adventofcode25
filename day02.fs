: digitLen ( n -- n )
    0 swap ( acc n )
    begin
        dup 0 <>
    while
            10 /
            swap 1+ swap
    repeat
    drop
;

: pow ( n k -- n^k )
    1 -rot ( acc n k )
    begin
        dup 0 <>
    while
            1- -rot ( k' acc n )
            dup -rot ( k' n acc n )
            * ( k' n acc' )
            -rot swap ( acc' k' n )
    repeat
    drop drop
;


: digitLen-tests
    assert( 3 digitLen 1 = )
    assert( 10 digitLen 2 = )
    assert( 35 digitLen 2 = )
    assert( 99 digitLen 2 = )
    assert( 100 digitLen 3 = )
    assert( 123 digitLen 3 = )
    assert( 13116184 digitLen 8 = )
    assert( 929862406 digitLen 9 = )
;

: pow-tests
    assert( 10 3 pow 1000 = )
;

: repeated? ( n -- f )
    \ n/10^(l/2) == n mod 10^(l/2)
    dup ( n n )
    digitLen 2 / ( n halflen )
    10 swap pow ( n 10^halflen )
    over over ( n 10^ n 10^ )
    mod -rot / ( nmod ndiv )
    =
;

: repeated?-tests
    assert( 11 repeated? )
    assert( 1212 repeated? )
    assert( 121 repeated? invert )
;

: count ( from to -- n )
    1+ swap 0 -rot ( acc limit from )
    ?do
        ( acc )
        i repeated?
        if i . i + then
    loop
;

: count-tests
    assert( 11 22 count 33 = )
    assert( 95 115 count 99 = )
;

: step ( acc from to -- acc )
    count +
;

: part1
0
989133 1014784
step 6948 9419
step 13116184 13153273
step 4444385428 4444484883
step 26218834 26376188
step 224020 287235
step 2893 3363
step 86253 115248
step 52 70
step 95740856 95777521
step 119 147
step 67634135 67733879
step 2481098640 2481196758
step 615473 638856
step 39577 47612
step 9444 12729
step 93 105
step 929862406 930001931
step 278 360
step 452131 487628
step 350918 426256
step 554 694
step 68482544 68516256
step 419357748 419520625
step 871 1072
step 27700 38891
step 26 45
step 908922 976419
step 647064 746366
step 9875192107 9875208883
step 3320910 3352143
step 1 19
step 373 500
step 4232151 4423599
step 1852 2355
step 850857 889946
step 4943 6078
step 74 92
step 4050 4876
    step
    cr cr ." part1 "
.
;
\ answer 18700015741

: repeated-in {: n k | divisor part -- f :}
    \ ." REPEATED( " n . k . ." )"
    10 k pow to divisor
    n divisor mod to part
    \ early-out: part can't start with zeros!
    part 10 k 1- pow < if false exit then

    n divisor / ( rest )
    \ ." DBG( " divisor . part . dup . ." )"
    begin
        divisor /mod ( part' rest' )
        swap part <> if drop false exit then
        ( rest' )
        dup 0<> while
    repeat
    drop
    true
;

: repeated-in-tests
    assert( 123123 3 repeated-in )
    assert( 123124 3 repeated-in invert )
    assert( 1231231 3 repeated-in invert )
    assert( 123123123 3 repeated-in )
    assert( 11 1 repeated-in )
    assert( 12 1 repeated-in invert )
    assert( 10101 1 repeated-in invert )
    assert( 10101 2 repeated-in invert )
;

: invalid? ( n -- f )
    dup digitLen 2 / 1+ 1 ( n halflen+1 1 )
    ?do
        dup i repeated-in ( n f )
        if unloop drop true exit then
    loop
    drop false exit
;

: invalid?-tests
    assert( 10101 invalid? invert )
    assert( 123123 invalid? )
    assert( 123193 invalid? invert )
    assert( 123123123 invalid? )
    assert( 123143123 invalid? invert )
    assert( 1231231231 invalid? invert )
    assert( 12341234 invalid? )
    assert( 12341235 invalid? invert )
;

: count2 ( from to -- n )
    1+ swap 0 -rot ( acc limit from )
    ?do
        ( acc )
        i invalid?
        if i . i + then
    loop
;

: count2-tests
    assert( 11 22 count2 33 = )
    assert( 95 115 count2 99 111 + = )
    assert( 998 1012 count2 999 1010 + = )
    assert( 2121212118 2121212124 count2 2121212121 = )
;

: step2 ( acc from to -- acc )
    count2 +
;

: part2
0
989133 1014784
step2 6948 9419
step2 13116184 13153273
step2 4444385428 4444484883
step2 26218834 26376188
step2 224020 287235
step2 2893 3363
step2 86253 115248
step2 52 70
step2 95740856 95777521
step2 119 147
step2 67634135 67733879
step2 2481098640 2481196758
step2 615473 638856
step2 39577 47612
step2 9444 12729
step2 93 105
step2 929862406 930001931
step2 278 360
step2 452131 487628
step2 350918 426256
step2 554 694
step2 68482544 68516256
step2 419357748 419520625
step2 871 1072
step2 27700 38891
step2 26 45
step2 908922 976419
step2 647064 746366
step2 9875192107 9875208883
step2 3320910 3352143
step2 1 19
step2 373 500
step2 4232151 4423599
step2 1852 2355
step2 850857 889946
step2 4943 6078
step2 74 92
step2 4050 4876
    step2
    cr cr ." part2 "
.
;
\ answer 20077272987

: example2
    0
    11 22 step2
    95 115 step2
    998 1012 step2
    1188511880 1188511890 step2
    222220 222224 step2
    1698522 1698528 step2
    446443 446449 step2
    38593856 38593862 step2
    565653 565659 step2
    824824821 824824827 step2
    2121212118 2121212124 step2

    cr cr ." example2 " .
;

: tests
    digitLen-tests
    pow-tests
    repeated?-tests
    count-tests
    repeated-in-tests
    invalid?-tests
    count2-tests
;
