using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// 字段流
/// </summary>
public class FieldStream
{
    public const int STREAM_DEFAULT_SIZE = 10240;

    // 缓冲区
    private byte[] _buffer;
    // 当前访问索引
    private int _currentPos;
    // 缓冲区大小
    private int _size;


    public FieldStream()
    {
        create(null, 0, STREAM_DEFAULT_SIZE);
    }

    public FieldStream(int size)
    {
        create(null, 0, size);
    }

    public FieldStream(byte[] stream, int begin)
    {
        create(stream, begin, stream.Length - begin);
    }

    /**
	    * 创建字段流
	    * @param stream 字节数组
	    * @param begin 起始位置
	    * @param size 大小
	    */
    public void create(byte[] stream, int begin, int size)
    {
        if (null == stream)
        {
            _buffer = new byte[size];
        }
        else
        {
            _buffer = stream;
        }
        _currentPos = begin;
        _size = size;
    }

    /**
	    * 转化为字节数组
	    * @return 字节数组
	    */
    public byte[] toBytes()
    {
        if (0 == _currentPos)
        {
            return null;
        }
        byte[] ret = new byte[_currentPos];
        Array.Copy(_buffer, 0, ret, 0, _currentPos);
        return ret;
    }

    public int currentPos()
    {
        return _currentPos;
    }

    public void currentPos(int pos)
    {
        _currentPos = pos;
    }

    public byte readByte()
    {
        _currentPos += 1;
        return _buffer[_currentPos - 1];
    }

    public short readShort()
    {
        AlignTo(2);

        _currentPos += 2;
        return bytesToShort(_buffer, _currentPos - 2);
    }

    //void read_CheckAlignment()
    //{
    //    while (_currentPos % 4 != 0)
    //    {
    //        readByte();
    //    }
    //}

    //void write_CheckAlignment()
    //{
    //    while (_currentPos % 4 != 0)
    //    {
    //        writeByte(0);
    //    }
    //}

    void AlignTo(int mod)
    {
        while (_currentPos % mod != 0)
        {
            ++_currentPos;
        }
    }

    public void writeShort(short number)
    {
        AlignTo(2);

        _buffer[_currentPos++] = (byte)(number & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 8) & 0xFF);

        if (_currentPos > _size)
        {
            // ("FieldStream.writeShort() overflow");
        }
    }

    public int readInt()
    {
        AlignTo(4);

        _currentPos += 4;
        return bytesToInt(_buffer, _currentPos - 4);
    }
    public uint readUint()
    {
        AlignTo(4);
        _currentPos += 4;
        return bytesToUint(_buffer, _currentPos - 4);
    }
    public long readLong()
    {
        AlignTo(8);

        _currentPos += 8;
        return bytesToLong(_buffer, _currentPos - 8);
    }

    public double readDouble()
    {
        AlignTo(8);

        _currentPos += 8;
        return bytesToDouble(_buffer, _currentPos - 8);
    }

    public void writeInt(int number)
    {
        AlignTo(4);

        _buffer[_currentPos++] = (byte)(number & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 8) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 16) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 24) & 0xFF);

        if (_currentPos > _size)
        {
            // ("FieldStream.writeShort() overflow");
        }
    }
    public void writeUint(uint number)
    {
        AlignTo(4);

        _buffer[_currentPos++] = (byte)(number & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 8) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 16) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 24) & 0xFF);

        if (_currentPos > _size)
        {
            // ("FieldStream.writeShort() overflow");
        }
    }
    public void writeLong(long number)
    {
        AlignTo(8);

        _buffer[_currentPos++] = (byte)(number & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 8) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 16) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 24) & 0xFF);

        _buffer[_currentPos++] = (byte)((number >> 32) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 40) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 48) & 0xFF);
        _buffer[_currentPos++] = (byte)((number >> 56) & 0xFF);

        if (_currentPos > _size)
        {
            // ("FieldStream.writeShort() overflow");
        }
    }

    public float readFloat()
    {
        AlignTo(4);

        _currentPos += 4;
        return bytesToFloat(_buffer, _currentPos - 4);
    }

    public void writeFloat(float number)
    {
        AlignTo(4);

        byte[] byTemp = floatToBytes(number);
        _buffer[_currentPos++] = byTemp[0];
        _buffer[_currentPos++] = byTemp[1];
        _buffer[_currentPos++] = byTemp[2];
        _buffer[_currentPos++] = byTemp[3];

        if (_currentPos > _size)
        {
            // ("FieldStream.writeShort() overflow");
        }
    }

    /**
	    * 读取字符串
	    * @param character 字符编码
	    * @return 字符串
	    */
    public String readString(String character)
    {
        int size = readShort();

        byte[] bytes = new byte[size];
        Array.Copy(_buffer, _currentPos, bytes, 0, size);
        _currentPos += size;
        try
        {
            if (character == "")
            {
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /**
	    * 读取字符串
	    * @return 字符串
	    */
    public String readString()
    {
        string s = readString("UTF-8").Replace(("|n"), ("\n")).Replace(("|r"), ("\r"));
        return s;
    }

    /**
	    * 添加字符串
	    * @param line 字符串
	    * @param character 字符编码
	    */
    public void writeString(String line, String character)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(line);
            short count = (short)bytes.Length;
            _buffer[_currentPos++] = (byte)(count & 0xFF);
            _buffer[_currentPos++] = (byte)((count >> 8) & 0xFF);
            Array.Copy(bytes, 0, _buffer, _currentPos, bytes.Length);
            _currentPos += bytes.Length;
            if (_currentPos > _size)
            {
                // ("FieldStream.writeShort() overflow");
            }
        }
        catch (Exception e)
        {
            // ("FieldStream.writeString(String line, String character) failed");
        }
    }

    /**
	    * 添加字符串
	    * @param line 字符串
	    */
    public void writeString(String line)
    {
        writeString(line, "UTF-8");
    }

    public void readBuff(ref byte[] buff, int len)
    {
        for (int i = 0; i < len; ++i)
        {
            buff[i] = readByte();
        }
    }

    public void writeBuff(byte[] buff, int len)
    {
        for (int i = 0; i < len; ++i)
        {
            writeByte(buff[i]);
        }
    }

    public string readFixedLenString(int len)
    {
        byte[] buff = new byte[len];
        readBuff(ref buff, len);
        return Regex.Replace(Encoding.UTF8.GetString(buff), "\0", "");
    }

    public void writeFixedLenString(string line, int len)
    {
        byte[] buff = new byte[len];

        if (!string.IsNullOrEmpty(line))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(line);

            for (int i = 0; i < len && i < bytes.Length; ++i)
            {
                buff[i] = (byte)bytes[i];
            }
        }

        writeBuff(buff, len);
    }

    /**
	    * 写入字节流
	    * @param bytes 字节流
	    */
    public void writeBytes(byte[] bytes)
    {
        Array.Copy(bytes, 0, _buffer, _currentPos, bytes.Length);
        _currentPos += bytes.Length;
    }

    public void writeByte(byte b)
    {
        writeBytes(new byte[] { b });
    }

    public static byte[] shortToBytes(short number)
    {
        byte[] bytes = new byte[2];
        bytes[0] = (byte)(number & 0xFF);
        bytes[1] = (byte)((number >> 8) & 0xFF);
        return bytes;
    }

    public static short bytesToShort(byte[] bytes, int index)
    {
        return (short)(((bytes[index + 1] << 8) | bytes[index] & 0xff));
    }

    public static byte[] intToBytes(int number)
    {
        byte[] bytes = new byte[4];
        bytes[0] = (byte)(number & 0xFF);
        bytes[1] = (byte)((number >> 8) & 0xFF);
        bytes[2] = (byte)((number >> 16) & 0xFF);
        bytes[3] = (byte)((number >> 24) & 0xFF);
        return bytes;
    }

    public static int bytesToInt(byte[] bytes, int index)
    {
        return (int)((((bytes[index + 3] & 0xff) << 24)
                | ((bytes[index + 2] & 0xff) << 16)
                | ((bytes[index + 1] & 0xff) << 8) | ((bytes[index] & 0xff) << 0)));
    }
    
    public static uint bytesToUint(byte[] bytes, int index)
    {
        return (uint)((((bytes[index + 3] & 0xff) << 24)
                | ((bytes[index + 2] & 0xff) << 16)
                | ((bytes[index + 1] & 0xff) << 8) | ((bytes[index] & 0xff) << 0)));
    }
    public static long bytesToLong(byte[] bytes, int index)
    {
        return (long)
            ((((long)(bytes[index + 7] & 0xff) << 56)
                | ((long)(bytes[index + 6] & 0xff) << 48)
                | ((long)(bytes[index + 5] & 0xff) << 40)
                | ((long)(bytes[index + 4] & 0xff) << 32)
                | ((long)(bytes[index + 3] & 0xff) << 24)
                | ((long)(bytes[index + 2] & 0xff) << 16)
                | ((long)(bytes[index + 1] & 0xff) << 8)
                | ((long)(bytes[index + 0] & 0xff) << 0)));
    }

    public static byte[] floatToBytes(float number)
    {
        return BitConverter.GetBytes(number);
    }

    public static float bytesToFloat(byte[] bytes, int index)
    {
        return BitConverter.ToSingle(bytes, index);
    }

    public static double bytesToDouble(byte[] bytes, int index)
    {
        return BitConverter.ToDouble(bytes, index);
    }
}
