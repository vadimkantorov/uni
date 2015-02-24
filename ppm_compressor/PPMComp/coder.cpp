#define _CRT_SECURE_NO_DEPRECATE
/*
 * Listing 2 -- coder.c
 *
 * This file contains the code needed to accomplish arithmetic
 * coding of a symbol.  All the routines in this module need
 * to know in order to accomplish coding is what the probabilities
 * and scales of the symbol counts are.  This information is
 * generally passed in a SYMBOL structure.
 *
 * This code was first published by Ian H. Witten, Radford M. Neal,
 * and John G. Cleary in "Communications of the ACM" in June 1987,
 * and has been modified slightly for this article.  The code
 * is  published here with permission.
 */

#include <stdio.h>
#include "coder.h"
#include "bitio.h"
#include "qofmath128.c"

/*
 * These four variables define the current state of the arithmetic
 * coder/decoder.  They are assumed to be 16 bits long.  Note that
 * by declaring them as short ints, they will actually be 16 bits
 * on most 80X86 and 680X0 machines, as well as VAXen.
 */
static unsigned int code;  /* The present input code value       */
static unsigned int low;   /* Start of the current code range    */
static unsigned int high;  /* End of the current code range      */
int underflow_bits;             /* Number of underflow bits pending   */

const unsigned int MAX_VAL = 0xffffffff;
const unsigned int LAST_BIT = 0x80000000;
const unsigned int NEXT_TO_LAST_BIT = 0x40000000;
const unsigned int NEXT_TO_LAST_BIT_MINUS_ONE = 0x3fffffff;

unsigned int SafeConvert(unsigned long long x)
{
  if (x > 0xffffffffULL)
    throw "Overflow";
  return static_cast<unsigned int>(x);
}

/*
 * This routine must be called to initialize the encoding process.
 * The high register is initialized to all 1s, and it is assumed that
 * it has an infinite string of 1s to be shifted into the lower bit
 * positions when needed.
 */
void initialize_arithmetic_encoder()
{
    low = 0;
    high = MAX_VAL;
    underflow_bits = 0;
}

char buf[100];

/*
 * This routine is called to encode a symbol.  The symbol is passed
 * in the SYMBOL structure as a low count, a high count, and a range,
 * instead of the more conventional probability ranges.  The encoding
 * process takes two steps.  First, the values of high and low are
 * updated to take into account the range restriction created by the
 * new symbol.  Then, as many bits as possible are shifted out to
 * the output stream.  Finally, high and low are stable again and
 * the routine returns.
 */
void encode_symbol( FILE *stream, SYMBOL *s )
{
    unsigned long long range;
    range = (unsigned long long) ( high-low ) + 1;
    high = SafeConvert(div128(mult128(range, s->high_count), s->scale).lo + low - 1);
    low = SafeConvert(div128(mult128(range, s->low_count), s->scale).lo + low);
/*
 * This loop turns out new bits until high and low are far enough
 * apart to have stabilized.
 */
    for ( ; ; )
    {
/*
 * If this test passes, it means that the MSDigits match, and can
 * be sent to the output stream.
 */
        if ( ( high & LAST_BIT ) == ( low & LAST_BIT ) )
        {
            output_bit( stream, high & LAST_BIT );
            while ( underflow_bits > 0 )
            {
                output_bit( stream, ~high & LAST_BIT );
                underflow_bits--;
            }
        }
/*
 * If this test passes, the numbers are in danger of underflow, because
 * the MSDigits don't match, and the 2nd digits are just one apart.
 */
        else if ( ( low & NEXT_TO_LAST_BIT ) && !( high & NEXT_TO_LAST_BIT ))
        {
            underflow_bits += 1;
            low &= NEXT_TO_LAST_BIT_MINUS_ONE;
            high |= NEXT_TO_LAST_BIT;
        }
        else
            return ;
        low <<= 1;
        high <<= 1;
        high |= 1;
    }
}

/*
 * At the end of the encoding process, there are still significant
 * bits left in the high and low registers.  We output two bits,
 * plus as many underflow bits as are necessary.
 */
void flush_arithmetic_encoder( FILE *stream )
{
    output_bit( stream, low & NEXT_TO_LAST_BIT );
    underflow_bits++;
    while ( underflow_bits-- > 0 )
        output_bit( stream, ~low & NEXT_TO_LAST_BIT );
}

/*
 * When decoding, this routine is called to figure out which symbol
 * is presently waiting to be decoded.  This routine expects to get
 * the current model scale in the s->scale parameter, and it returns
 * a count that corresponds to the present floating point code:
 *
 *  code = count / s->scale
 */
unsigned long long get_current_count( SYMBOL *s )
{
    unsigned long long range = (unsigned long long) ( high - low ) + 1;
    unsigned long long z = (unsigned long long)(code - low) + 1;
    qofint128 one = {0, 1, 1, 0};
    qofint128 a = mult128(z, s->scale);
    qofint128 b = add128(a, one);
    qofint128 c = div128(b, range);
    return c.lo;
}

/*
 * This routine is called to initialize the state of the arithmetic
 * decoder.  This involves initializing the high and low registers
 * to their conventional starting values, plus reading the first
 * 16 bits from the input stream into the code value.
 */
void initialize_arithmetic_decoder( FILE *stream )
{
    int i;

    code = 0;
    for ( i = 0 ; i < 32 ; i++ )
    {
        code <<= 1;
        code += input_bit( stream );
    }
    low = 0;
    high = MAX_VAL;
}

/*
 * Just figuring out what the present symbol is doesn't remove
 * it from the input bit stream.  After the character has been
 * decoded, this routine has to be called to remove it from the
 * input stream.
 */
void remove_symbol_from_stream( FILE *stream, SYMBOL *s )
{
    unsigned long long range;
    range = (unsigned long long) ( high-low ) + 1;
    high = SafeConvert(div128(mult128(range, s->high_count), s->scale).lo + low - 1);
    low = SafeConvert(div128(mult128(range, s->low_count), s->scale).lo + low);
/*
 * Next, any possible bits are shipped out.
 */
    for ( ; ; )
    {
/*
 * If the MSDigits match, the bits will be shifted out.
 */
        if ( ( high & LAST_BIT ) == ( low & LAST_BIT ) )
        {
        }
/*
 * Else, if underflow is threatining, shift out the 2nd MSDigit.
 */
        else if ((low & NEXT_TO_LAST_BIT) == NEXT_TO_LAST_BIT  && (high & NEXT_TO_LAST_BIT) == 0 )
        {
            code ^= NEXT_TO_LAST_BIT;
            low   &= NEXT_TO_LAST_BIT_MINUS_ONE;
            high  |= NEXT_TO_LAST_BIT;
        }
/*
 * Otherwise, nothing can be shifted out, so I return.
 */
        else
            return;
        low <<= 1;
        high <<= 1;
        high |= 1;
        code <<= 1;
        code += input_bit( stream );
    }
}


