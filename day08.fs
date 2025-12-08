Create data 3000 cells allot
0 Value n-points

: skip-char ( ccc len -- ccc+1 len-1 )
    1- swap 1+ swap
;

: parse-number ( ccc len -- ccc len u )
    0 0 2swap ( ud ccc len )
    >number ( ud ccc len )
    2swap ( ccc len ud )
    drop
;

: parse-input ( ptr len -- )
    slurp-file
    0 to n-points
    begin
        parse-number
        data n-points th!
        n-points 1+ to n-points
        skip-char
        dup 0>
    while
    repeat
    assert( n-points 3 mod 0= )
    n-points 3 / to n-points
;

: get-coord ( axis i -- )
    3 * + data swap th@
;

: show ( -- )
    cr
    n-points 0 +do
        0 i get-coord .
        1 i get-coord .
        2 i get-coord .
        cr
    loop
;

: dist {: i j -- u :}
    0 i get-coord 0 j get-coord - dup *
    1 i get-coord 1 j get-coord - dup *
    2 i get-coord 2 j get-coord - dup *
;
