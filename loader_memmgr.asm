org 7c00h
max_blocks = 10
max_offset = 10000

start:
	cld
	xor	ax, ax
	mov	ds, ax
	mov	ss, ax
	mov	es, ax
	mov	sp, 7c00h

	cwd
.reread:
	mov	cx, 0002h
	mov	ax, 0201h
	mov	bx, 7c00h+200h
	int	13h
	jc	.reread
	
	mov dword [counts], 0
	mov [offsets], 0
	mov [offsets+2], max_offset
	
nxt_command:
	call	scan_keyboard

	push	nxt_command
	mov	bx, kb_buffer
	cmp	dword [bx], 'new '
	je	new_command
	cmp	dword [bx], 'del '
	je	del_command
	cmp	dword [bx], 'list'
	je	lst_command
	cmp	dword [bx], 'exit'
	je	dos_exit
invalid_command:
	mov	dx, error_string
	call	print
	ret ;jmp nxt_command	

;bx - адрес строки
;cx - результат
read_int:
	cld
	xor ax, ax
	xor cx, cx
	xor dx, dx
	
	mov si, bx
	mov di, 10
.read_int_step:
	lodsb
	or al, al
	jz .read_int_done
	sub ax, '0'
	xchg cx, ax
	imul di
	add ax, cx
	xchg cx, ax
	jmp .read_int_step
.read_int_done:
	ret
		
;ax - адрес первого элемента для сдвига
;bx - адрес последнего элемента для сдвига
array_shift_right:
	std
	mov si, bx
	mov di, bx
	add di, 2
.asr_step:
	movsw
	cmp si, ax
	jge .asr_step
	ret

;ax - адрес первого элемента для сдвига
;bx - адрес последнего элемента для сдвига	
array_shift_left:
	cld
	mov si, ax
	mov di, ax
	sub di, 2
.asl_step:
	movsw
	cmp si, bx
	jle .asl_step
	ret

;cx - индекс
;ax [выход]- разбежка
window:
	push bx
	push cx
	
	xor ax, ax
	shl cx, 1
	mov bx, offsets
	add bx, cx
	add ax, [bx]
	
	sub cx, 2
	mov bx, offsets
	add bx, cx
	sub ax, [bx]
	
	mov bx, counts
	add bx, cx
	sub ax, [bx]
	pop cx
	pop bx
	ret

new_command:
	mov bx, kb_buffer+4
	call read_int
	
	mov bx, cx
	mov cx, [last_block]

	mov dx, -1
.nc_step:
	call window
		
	cmp ax, bx
	jb .nc_step_end
	
	cmp dx, -1
	je .nc_update
	
	mov bp, ax
	push cx
	mov cx, dx
	call window
	pop cx
	
	cmp ax, bp ; bp - новое окно, ax - старое окно
	jb .nc_step_end

.nc_update:
	mov dx, cx
	
.nc_step_end:
	loop .nc_step
	
	cmp dx, -1
	je .nc_err
	
	; вставить новый count
	push bx
	mov ax, dx
	shl ax, 1
	add ax, counts
	
	mov bx, [last_block]
	shl bx, 1
	add bx, counts
	
	call array_shift_right
	
	mov bp, dx
	shl bp, 1
	add bp, counts
	
	pop bx
	mov [bp], bx
	
	; вставить новый offset
	mov ax, dx
	shl ax, 1
	add ax, offsets
	
	mov bx, [last_block]
	shl bx, 1
	add bx, offsets
	
	call array_shift_right
	
	dec dx
	
	mov bp, dx
	shl bp, 1
	add bp, offsets
	mov ax, [bp]
	
	mov bp, dx
	shl bp, 1
	add bp, counts
	add ax, [bp]
	
	inc dx
	mov bp, dx
	shl bp, 1
	add bp, offsets
	mov [bp], ax
	
	inc [last_block]

	mov dx, new_succeded
	call print
	call printn
	mov dx, new_line
	call print
	jmp .new_command_done
.nc_err:
	mov dx, insufficient_memory
	call print
.new_command_done:
	ret

del_command:
	mov bx, kb_buffer+4
	call read_int
	
	mov bx, cx
	mov cx, [last_block]
.del_step:
	mov bp, cx
	shl bp, 1
	add bp, offsets
	cmp [bp], bx
	je .del_found
	loop .del_step
	mov dx, del_cannot_find
	call print
	ret
.del_found:
	inc cx
	mov ax, cx
	shl ax, 1
	add ax, offsets
	
	mov bx, [last_block]
	shl bx, 1
	add bx, offsets
	call array_shift_left
	
	mov ax, cx
	shl ax, 1
	add ax, counts
	
	mov bx, [last_block]
	shl bx, 1
	add bx, counts
	call array_shift_left
	dec [last_block]
	
	mov dx, del_succeded
	call print
	ret

lst_command:
	mov cx, [last_block]
	dec cx
	cmp cx, 0
	je .lst_done
.lst_step:
	mov bp, cx
	shl bp, 1
	add bp, offsets
	mov ax, [bp]
	call printn
	
	mov dx, left_bracket
	call print
	
	mov bp, cx
	shl bp, 1
	add bp, counts
	mov ax, [bp]
	call printn
	
	mov dx, right_bracket
	call print
	
	mov dx, new_line
	call print
	
	loop .lst_step
.lst_done:
	ret

printn:
	pusha
	mov	bx, pstr
divn:
	dec	bx
	xor	dx, dx
	mov	cx, 10
	div	cx
	add	dl, '0'
	mov	[bx], dl
	test	ax, ax
	jnz	divn	

	mov	dx, bx
	call print
	popa
	retn
	
print:
	cld
	pusha
	mov	si, dx
.nxt:	lodsb
	test	al, al
	jz	.szero
	cmp	al, 10
	jne	.nrml
	mov	al, 13
.nrml:	call	put_character
	jmp	.nxt
.szero:
	popa
	ret	
		
scan_keyboard:
	cld
	mov	di, kb_buffer
.nxt:	xor	ah, ah
	int	16h
	call	put_character
	stosb
	cmp	[end_scancode], ah
	jne	.nxt
	dec	di
	xor	al, al
	stosb
	ret
end_scancode db 1ch

put_character:
	pusha
	mov	ah, 0eh
	mov	bl, 0fh
	int	10h
	popa
	cmp	al, 13
	jne	.noenter
	mov	al, 10
	call	put_character
.noenter:
	ret

dos_exit:
	mov     ax, 4c00h
    int     21h
	ret
	
	
error_string	db 'invalid command',10,0
insufficient_memory	db 'insufficient memory',10,0
new_line		db 10,0
left_bracket	db '(',0
right_bracket	db ')',0
new_succeded	db 'hurray! your lovely address is: ',0
del_succeded	db 'reclaimed!',10,0
del_cannot_find	db 'cannot find requested address',10,0
offsets			dw max_blocks dup(0)
counts			dw max_blocks dup(0)
last_block		dw 1
dw 10 dup(' ')
pstr	dw 0
kb_buffer db 7c00h+1024-$ dup(0)
db 200h*20 dup(0)
align 200h