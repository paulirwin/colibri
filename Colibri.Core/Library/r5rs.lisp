#| TODO: reduce duplication with other library files |#
(define (exp z) (Math/Pow Math/E z))
(define (sin z) (Math/Sin z))
(define (cos z) (Math/Cos z))
(define (tan z) (Math/Tan z))
(define (asin z) (Math/Asin z))
(define (acos z) (Math/Acos z))
(define atan (lambda z (if (= (length z) 2) (Math/Atan2 (car z) (cadr z)) (Math/Atan (car z)))))

(define assoc
	(lambda (obj alist . compare)
		(let ((predicate (if (> (length compare) 0) (car compare) equal?)))
			(if (eq? alist '()) #f
				(if (predicate obj (caar alist)) (car alist) (assoc obj (cdr alist) predicate))))))
(define assq (lambda (obj alist) (assoc obj alist eq?)))
(define assv (lambda (obj alist) (assoc obj alist eqv?)))

(define (caar x) (car (car x)))
(define (cadr x) (car (cdr x)))
(define (cdar x) (cdr (car x)))
(define (cddr x) (cdr (cdr x)))

(define (caaar x) (car (car (car x))))
(define (caadr x) (car (car (cdr x))))
(define (cadar x) (car (cdr (car x))))
(define (caddr x) (car (cdr (cdr x))))
(define (cdaar x) (cdr (car (car x))))
(define (cdadr x) (cdr (car (cdr x))))
(define (cddar x) (cdr (cdr (car x))))
(define (cdddr x) (cdr (cdr (cdr x))))

(define (caaaar x) (car (car (car (car x)))))
(define (caaadr x) (car (car (car (cdr x)))))
(define (caadar x) (car (car (cdr (car x)))))
(define (caaddr x) (car (car (cdr (cdr x)))))
(define (cadaar x) (car (cdr (car (car x)))))
(define (cadadr x) (car (cdr (car (cdr x)))))
(define (caddar x) (car (cdr (cdr (car x)))))
(define (cadddr x) (car (cdr (cdr (cdr x)))))
(define (cdaaar x) (cdr (car (car (car x)))))
(define (cdaadr x) (cdr (car (car (cdr x)))))
(define (cdadar x) (cdr (car (cdr (car x)))))
(define (cdaddr x) (cdr (car (cdr (cdr x)))))
(define (cddaar x) (cdr (cdr (car (car x)))))
(define (cddadr x) (cdr (cdr (car (cdr x)))))
(define (cdddar x) (cdr (cdr (cdr (car x)))))
(define (cddddr x) (cdr (cdr (cdr (cdr x)))))

(define (call-with-input-file file proc) (call-with-port (open-input-file file) proc))
(define (call-with-output-file file proc) (call-with-port (open-output-file file) proc))

(define (odd? x) (= (% x 2) 1))
(define (even? x) (= (% x 2) 0))
(define (positive? x) (> x 0))
(define (negative? x) (< x 0))
(define (zero? x) (= x 0))

(define list-tail
	(lambda (x k)
		(if (zero? k)
		x
		(list-tail (cdr x) (- k 1)))))
(define list-ref
	(lambda (x k) (car (list-tail x k))))
	
(define member
    (lambda (obj list . compare)
        (let ((predicate (if (> (length compare) 0) (car compare) equal?)))
            (if (eq? list '()) #f
                (if (predicate obj (car list)) list (member obj (cdr list) predicate))))))
(define memq (lambda (obj list) (member obj list eq?)))
(define memv (lambda (obj list) (member obj list eqv?)))