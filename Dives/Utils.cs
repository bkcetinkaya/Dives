using System;
using System.Collections.Generic;
using System.Text;

namespace Dives
{
    public class Utils
    {

        // int is 32 bytes. We hold the header information in an integer (int messageSize in the parameters). By shifting the bits to the 24,16,8, times,
        // we put the leftern most 8 bits to the first element of the byte array. (This is big endian, most significant bits are on the left.)
        // Let's say messageSize parameter is 12 which is 00000000 00000000 00000000 00001100. Shifting the bits to the right 24 times will put the first 8 bytes at the end.
        // So bytes in the integer, and the bytes in the byte array are going to be like this at each step: 
        // 00000000 00000000 00000000 00000000 -  00000000 <-- first element of byte array
        // 00000000 00000000 00000000 00000000 -  00000000 <-- second element of byte array and so on
        // 00000000 00000000 00000000 00000000 -  00000000
        // 00000000 00000000 00000000 00001100 -  00001100
        // Note: casting 32 bit value to 8 bit value will only copy the rigthern most 8 bit.
        public static void IntToBytesBigEndianNonAlloc(int messageSize , ref byte[] payload, int position)
        {
            payload[position + 0 ] = (byte)(messageSize >> 24);
            payload[position + 1 ] = (byte)(messageSize >> 16);
            payload[position + 2 ] = (byte)(messageSize >> 8);
            payload[position + 3 ] = (byte)(messageSize);
        }
    }
}
