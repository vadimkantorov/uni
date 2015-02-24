
sseg segment stack
dw 1000
sseg ends

dseg segment
msg     db 'division by zero ',13,10,'$'
fmsg     db 'fake message$'
mend     db 'exit',13,10,'$'
save_off dw ?
save_seg dw ?
dseg ends

cseg segment
     assume     cs:cseg, ds:dseg, es:nothing, ss:sseg
start proc
     mov     ax, dseg
     mov     ds, ax
     mov     ax, sseg
     mov     ss, ax

     xor     ax, ax
     mov      es, ax
     
     mov     dx, offset div_int
     mov     cx, cs
     cli
     xchg     dx, [es:0]
     xchg     cx, [es:2]
     sti

     mov     save_off, dx
     mov     save_seg, cx
     
     div     ax

     mov     ah, 9     
     mov     dx, offset fmsg
     int     21h     
finish:
     mov     ah, 9     
     mov     dx, offset mend
     int     21h     

     mov     ax, 4c00h
     int     21h
start endp

div_int proc
     push     ax
     push     dx
     push     ds
     mov     ax, dseg
     mov     ds, ax
     mov     dx, save_off
     mov     ax, save_seg
     mov     [es:0], dx
     mov     [es:2], ax

     mov     ah, 9
     mov     dx, offset msg
     int     21h
     pop     ds
     pop     dx
     push     bp
     mov     bp, sp
     mov     ax, offset finish
     mov     word ptr [bp+4], ax
     pop     bp
     pop     ax
     iret
div_int endp
cseg ends

end start
