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


: log10-tests
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
    \ assert( 1852-2355
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

: tests
    log10-tests
    pow-tests
    repeated?-tests
    count-tests
;
