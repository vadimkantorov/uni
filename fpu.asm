.286
.287
dseg segment
	x	dd 2.5
	a	dd 10.2
	b	dd 20.5
	res dd ?

	mab	db 'between a,b$'
	ma	db 'less than a$'
	mb	db 'greater than b$'
dseg ends

cseg segment
	assume	cs:cseg, ds:dseg
start proc
	mov		ax, dseg
    mov		ds, ax
	finit
	fld	x
	fcom	a
	fstsw	ax
	sahf
	jb	lss_a
	fcom	b
	fstsw	ax
	sahf
	ja	gtt_b
btw_ab:					;x*x*sqrt x
	mov	dx, offset mab
	fst st(1)
	fst st(2)
	fsqrt
	fmulp
	fmulp
	jmp	endr
gtt_b:					;abs x
	mov	dx, offset mb
	fabs
	jmp	endr
lss_a:					;2^x-1
	mov	dx, offset ma
	f2xm1				
endr:
	fst res
	mov  ax, offset res
	mov	ah, 9
	int	21h
	mov ax,4C00h
	int	21h
start endp
cseg ends

end	start