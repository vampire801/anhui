using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Common
{
    public class XConvert
    {
        static public byte[] ToByte(object o)
        {
            MemoryStream ms = new MemoryStream();
            int alignment = 1; //max size of object alignment
            int align = 1; //max size of local alignment

            int size = ToByte(o.GetType(), o, ms, ref alignment, ref align);

            if (0 != size % alignment)
            {
                size = (size / alignment + 1) * alignment;
                ms.SetLength(size);
                ms.Position = size;
            }

            byte[] b = (ms.Length > 0) ? ms.ToArray() : null;

            ms.Close();
            ms = null;

            return b;
        }

        static public T ToObject<T>(byte[] buffer)
        {
            int offset = 0;
            return ToObject<T>(buffer, ref offset);
        }

        static public T ToObject<T>(byte[] buffer, ref int offset)
        {
            Type t = typeof(T);

            object o = Activator.CreateInstance(t);

            int alignment = 1;

            ToObject(t, o, buffer, ref offset, ref alignment);

            if (0 != offset % alignment)
            {
                offset = (offset / alignment + 1) * alignment;
            }

            return (T)o;
        }

        static int ToByte(Type t, object o, MemoryStream ms, ref int alignment, ref int align)
        {
            int size = 0; //local size
            int pack = (0 == t.StructLayoutAttribute.Pack) ? 8 : t.StructLayoutAttribute.Pack; //pack size of object o

            try
            {
                foreach (FieldInfo fi in t.GetFields())
                {
                    Type ft = fi.FieldType;

                    if ((ft.IsArray) || (ft.Equals(typeof(String))))
                    {
                        object[] objs = fi.GetCustomAttributes(false);
                        if (null == objs)
                        {
                            throw new Exception(t.Name + "::" + fi.Name + " GetCustomAttributes Error !");
                        }

                        MarshalAsAttribute attr = (MarshalAsAttribute)objs[0];

                        if (ft.Equals(typeof(String)))
                        {
                            String str = (String)fi.GetValue(o);
                            if (null != str)
                            {
                                //Byte[] v = System.Text.Encoding.GetEncoding("gb2312").GetBytes((String)fi.GetValue(o));
                                Byte[] v = System.Text.Encoding.UTF8.GetBytes((String)fi.GetValue(o));
                                ms.Write(v, 0, v.Length);
                            }
                            size += sizeof(Byte) * attr.SizeConst;
                            ms.SetLength(size);
                            ms.Position = ms.Length;
                        }
                        else if (ft.Equals(typeof(SByte[])))
                        {
                            SByte[] v = (SByte[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new SByte[attr.SizeConst];
                            }
                            for (int i = 0; i < v.Length; i++)
                            {
                                ms.WriteByte((Byte)v[i]);
                            }
                            size += sizeof(SByte) * attr.SizeConst;
                            ms.SetLength(size);
                            ms.Position = ms.Length;
                        }
                        else if (ft.Equals(typeof(Byte[])))
                        {
                            Byte[] v = (Byte[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Byte[attr.SizeConst];
                            }
                            ms.Write(v, 0, v.Length);
                            size += sizeof(Byte) * attr.SizeConst;
                            ms.SetLength(size);
                            ms.Position = ms.Length;
                        }
                        else if (ft.Equals(typeof(Int16[])))
                        {
                            if (align < sizeof(Int16))
                            {
                                align = sizeof(Int16);
                            }

                            int align1 = (pack > sizeof(Int16)) ? sizeof(Int16) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            Int16[] v = (Int16[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Int16[attr.SizeConst];
                            }
                            foreach (Int16 i in v)
                            {
                                byte[] b = BitConverter.GetBytes(i);
                                ms.Write(b, 0, b.Length);
                            }
                            size += sizeof(Int16) * attr.SizeConst;
                        }
                        else if (ft.Equals(typeof(UInt16[])))
                        {
                            if (align < sizeof(UInt16))
                            {
                                align = sizeof(UInt16);
                            }

                            int align1 = (pack > sizeof(UInt16)) ? sizeof(UInt16) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            UInt16[] v = (UInt16[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new UInt16[attr.SizeConst];
                            }
                            foreach (UInt16 i in v)
                            {
                                byte[] b = BitConverter.GetBytes(i);
                                ms.Write(b, 0, b.Length);
                            }
                            size += sizeof(UInt16) * attr.SizeConst;
                        }
                        else if (ft.Equals(typeof(Int32[])))
                        {
                            if (align < sizeof(Int32))
                            {
                                align = sizeof(Int32);
                            }

                            int align1 = (pack > sizeof(Int32)) ? sizeof(Int32) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            Int32[] v = (Int32[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Int32[attr.SizeConst];
                            }
                            foreach (Int32 i in v)
                            {
                                byte[] b = BitConverter.GetBytes(i);
                                ms.Write(b, 0, b.Length);
                            }
                            size += sizeof(Int32) * attr.SizeConst;
                        }
                        else if (ft.Equals(typeof(UInt32[])))
                        {
                            if (align < sizeof(UInt32))
                            {
                                align = sizeof(UInt32);
                            }

                            int align1 = (pack > sizeof(UInt32)) ? sizeof(UInt32) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            UInt32[] v = (UInt32[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new UInt32[attr.SizeConst];
                            }
                            foreach (UInt32 i in v)
                            {
                                byte[] b = BitConverter.GetBytes(i);
                                ms.Write(b, 0, b.Length);
                            }
                            size += sizeof(UInt32) * attr.SizeConst;
                        }
                        else if (ft.Equals(typeof(Int64[])))
                        {
                            if (align < sizeof(Int64))
                            {
                                align = sizeof(Int64);
                            }

                            int align1 = (pack > sizeof(Int64)) ? sizeof(Int64) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            Int64[] v = (Int64[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Int64[attr.SizeConst];
                            }
                            foreach (Int64 i in v)
                            {
                                byte[] b = BitConverter.GetBytes(i);
                                ms.Write(b, 0, b.Length);
                            }
                            size += sizeof(Int64) * attr.SizeConst;
                        }
                        else if (ft.Equals(typeof(UInt64[])))
                        {
                            if (align < sizeof(UInt64))
                            {
                                align = sizeof(UInt64);
                            }

                            int align1 = (pack > sizeof(UInt64)) ? sizeof(UInt64) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            UInt64[] v = (UInt64[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new UInt64[attr.SizeConst];
                            }
                            foreach (UInt64 i in v)
                            {
                                byte[] b = BitConverter.GetBytes(i);
                                ms.Write(b, 0, b.Length);
                            }
                            size += sizeof(UInt64) * attr.SizeConst;
                        }
                        else if (ft.IsClass)
                        {
                            object[] v = (object[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = (object[])Activator.CreateInstance(ft, attr.SizeConst);
                                Type type = Type.GetType(ft.FullName.Replace("[]", ""));
                                for (int i = 0; i < attr.SizeConst; i++)
                                {
                                    v[i] = Activator.CreateInstance(type);
                                }
                            }
                            foreach (object ob in v)
                            {
                                int localalign = 1;
                                MemoryStream stream = new MemoryStream();
                                int size1 = ToByte(ob.GetType(), ob, stream, ref alignment, ref localalign);
                                if (align < localalign)
                                {
                                    align = localalign;
                                }
                                int align1 = (pack > localalign) ? localalign : pack; //member alignment
                                if (0 != size % align1)
                                {
                                    size = (size / align1 + 1) * align1;
                                    ms.SetLength(size);
                                    ms.Position = ms.Length;
                                }
                                size += size1;
                                stream.WriteTo(ms);
                            }
                        }
                        else
                        {
                            throw new Exception(t.Name + "::" + fi.Name + " not support typeof " + ft.Name);
                        }
                    }
                    else if (ft.IsValueType)
                    {
                        if (ft.Equals(typeof(SByte)))
                        {
                            ms.WriteByte((Byte)(SByte)fi.GetValue(o));
                            size += sizeof(SByte);
                        }
                        else if (ft.Equals(typeof(Byte)))
                        {
                            ms.WriteByte((Byte)fi.GetValue(o));
                            size += sizeof(Byte);
                        }
                        else if (ft.Equals(typeof(Int16)))
                        {
                            if (align < sizeof(Int16))
                            {
                                align = sizeof(Int16);
                            }

                            int align1 = (pack > sizeof(Int16)) ? sizeof(Int16) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            byte[] b = BitConverter.GetBytes((Int16)fi.GetValue(o));
                            ms.Write(b, 0, b.Length);
                            size += sizeof(Int16);
                        }
                        else if (ft.Equals(typeof(UInt16)))
                        {
                            if (align < sizeof(UInt16))
                            {
                                align = sizeof(UInt16);
                            }

                            int align1 = (pack > sizeof(UInt16)) ? sizeof(UInt16) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            byte[] b = BitConverter.GetBytes((UInt16)fi.GetValue(o));
                            ms.Write(b, 0, b.Length);
                            size += sizeof(UInt16);
                        }
                        else if (ft.Equals(typeof(Int32)))
                        {
                            if (align < sizeof(Int32))
                            {
                                align = sizeof(Int32);
                            }

                            int align1 = (pack > sizeof(Int32)) ? sizeof(Int32) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            byte[] b = BitConverter.GetBytes((Int32)fi.GetValue(o));
                            ms.Write(b, 0, b.Length);
                            size += sizeof(Int32);
                        }
                        else if (ft.Equals(typeof(UInt32)))
                        {
                            if (align < sizeof(UInt32))
                            {
                                align = sizeof(UInt32);
                            }

                            int align1 = (pack > sizeof(UInt32)) ? sizeof(UInt32) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            byte[] b = BitConverter.GetBytes((UInt32)fi.GetValue(o));
                            ms.Write(b, 0, b.Length);
                            size += sizeof(UInt32);
                        }
                        else if (ft.Equals(typeof(Int64)))
                        {
                            if (align < sizeof(Int64))
                            {
                                align = sizeof(Int64);
                            }

                            int align1 = (pack > sizeof(Int64)) ? sizeof(Int64) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            byte[] b = BitConverter.GetBytes((Int64)fi.GetValue(o));
                            ms.Write(b, 0, b.Length);
                            size += sizeof(Int64);
                        }
                        else if (ft.Equals(typeof(UInt64)))
                        {
                            if (align < sizeof(UInt64))
                            {
                                align = sizeof(UInt64);
                            }

                            int align1 = (pack > sizeof(UInt64)) ? sizeof(UInt64) : pack; //member alignment

                            if (0 != size % align1)
                            {
                                size = (size / align1 + 1) * align1;
                                ms.SetLength(size);
                                ms.Position = ms.Length;
                            }
                            byte[] b = BitConverter.GetBytes((UInt64)fi.GetValue(o));
                            ms.Write(b, 0, b.Length);
                            size += sizeof(UInt64);
                        }
                        else
                        {
                            throw new Exception(t.Name + "::" + fi.Name + " not support typeof " + ft.Name);
                        }
                    }
                    else if (ft.IsClass)
                    {
                        int localalign = 1;
                        object v = fi.GetValue(o);
                        if (null == v)
                        {
                            v = Activator.CreateInstance(ft);
                        }
                        MemoryStream stream = new MemoryStream();
                        int size1 = ToByte(ft, v, stream, ref alignment, ref localalign);
                        if (align < localalign)
                        {
                            align = localalign;
                        }
                        int align1 = (pack > localalign) ? localalign : pack; //member alignment
                        if (0 != size % align1)
                        {
                            size = (size / align1 + 1) * align1;
                            ms.SetLength(size);
                            ms.Position = ms.Length;
                        }
                        size += size1;
                        stream.WriteTo(ms);
                    }
                    else
                    {
                        throw new Exception("not support typeof " + t.Name);
                    }
                }

                if (alignment < align)
                {
                    alignment = align;
                }

                int align2 = (pack > align) ? align : pack; //local alignment

                if (0 != size % align2)
                {
                    size = (size / align2 + 1) * align2;
                    ms.SetLength(size);
                    ms.Position = ms.Length;
                }
            }
            catch (Exception e)
            {
                DEBUG.Exception("" + e.Message);
            }

            return size;
        }

        static int GetAlignSize(Type t)
        {
            int align = 1;

            try
            {
                foreach (FieldInfo fi in t.GetFields())
                {
                    Type ft = fi.FieldType;

                    if ((ft.Equals(typeof(SByte[]))) || (ft.Equals(typeof(SByte))) ||
                        (ft.Equals(typeof(Byte[]))) || (ft.Equals(typeof(Byte))) || (ft.Equals(typeof(String))))
                    {

                    }
                    else if ((ft.Equals(typeof(Int16[]))) || (ft.Equals(typeof(Int16))) ||
                      (ft.Equals(typeof(UInt16[]))) || (ft.Equals(typeof(UInt16))))
                    {
                        if (align < sizeof(Int16))
                        {
                            align = sizeof(Int16);
                        }
                    }
                    else if ((ft.Equals(typeof(Int32[]))) || (ft.Equals(typeof(Int32))) ||
                      (ft.Equals(typeof(UInt32[]))) || (ft.Equals(typeof(UInt32))))
                    {
                        if (align < sizeof(Int32))
                        {
                            align = sizeof(Int32);
                        }
                    }
                    else if ((ft.Equals(typeof(Int64[]))) || (ft.Equals(typeof(Int64))) ||
                      (ft.Equals(typeof(UInt64[]))) || (ft.Equals(typeof(UInt64))))
                    {
                        if (align < sizeof(Int64))
                        {
                            align = sizeof(Int64);
                        }
                    }
                    else if (ft.IsClass)
                    {
                        int align1 = GetAlignSize(ft);
                        if (align < align1)
                        {
                            align = align1;
                        }
                    }
                    else
                    {
                        throw new Exception(t.Name + "::" + fi.Name + " not support typeof " + ft.Name);
                    }
                }
            }
            catch (Exception e)
            {
                DEBUG.Exception("" + e.Message);
            }

            return align;
        }

        static void ToObject(Type t, object o, byte[] buffer, ref int offset, ref int alignment)
        {
            int align = 1; //local alignment
            int pack = (0 == t.StructLayoutAttribute.Pack) ? 8 : t.StructLayoutAttribute.Pack; //pack size of object o

            try
            {
                foreach (FieldInfo fi in t.GetFields())
                {
                    Type ft = fi.FieldType;

                    if ((ft.IsArray) || (ft.Equals(typeof(String))))
                    {
                        object[] objs = fi.GetCustomAttributes(false);
                        if (null == objs)
                        {
                            throw new Exception(t.Name + "::" + fi.Name + " GetCustomAttributes Error !");
                        }

                        MarshalAsAttribute attr = (MarshalAsAttribute)objs[0];

                        if (ft.Equals(typeof(String)))
                        {

                            //fi.SetValue(o, System.Text.Encoding.GetEncoding("gb2312").GetString(buffer, offset, attr.SizeConst));
                            fi.SetValue(o, System.Text.Encoding.UTF8.GetString(buffer, offset, attr.SizeConst));
                            offset += attr.SizeConst;
                        }
                        else if (ft.Equals(typeof(SByte[])))
                        {
                            SByte[] v = (SByte[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new SByte[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset++)
                            {
                                v[i] = (SByte)buffer[offset];
                            }
                        }
                        else if (ft.Equals(typeof(Byte[])))
                        {
                            Byte[] v = (Byte[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Byte[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset++)
                            {
                                v[i] = buffer[offset];
                            }
                        }
                        else if (ft.Equals(typeof(Int16[])))
                        {
                            if (align < sizeof(Int16))
                            {
                                align = sizeof(Int16);
                            }

                            int align1 = (pack > sizeof(Int16)) ? sizeof(Int16) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            Int16[] v = (Int16[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Int16[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset += sizeof(Int16))
                            {
                                v[i] = BitConverter.ToInt16(buffer, offset);
                            }
                        }
                        else if (ft.Equals(typeof(UInt16[])))
                        {
                            if (align < sizeof(UInt16))
                            {
                                align = sizeof(UInt16);
                            }

                            int align1 = (pack > sizeof(UInt16)) ? sizeof(UInt16) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            UInt16[] v = (UInt16[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new UInt16[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset += sizeof(UInt16))
                            {
                                v[i] = BitConverter.ToUInt16(buffer, offset);
                            }
                        }
                        else if (ft.Equals(typeof(Int32[])))
                        {
                            if (align < sizeof(Int32))
                            {
                                align = sizeof(Int32);
                            }

                            int align1 = (pack > sizeof(Int32)) ? sizeof(Int32) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            Int32[] v = (Int32[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Int32[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset += sizeof(Int32))
                            {
                                v[i] = BitConverter.ToInt32(buffer, offset);
                            }
                        }
                        else if (ft.Equals(typeof(UInt32[])))
                        {
                            if (align < sizeof(UInt32))
                            {
                                align = sizeof(UInt32);
                            }

                            int align1 = (pack > sizeof(UInt32)) ? sizeof(UInt32) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            UInt32[] v = (UInt32[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new UInt32[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset += sizeof(UInt32))
                            {
                                v[i] = BitConverter.ToUInt32(buffer, offset);
                            }
                        }
                        else if (ft.Equals(typeof(Int64[])))
                        {
                            if (align < sizeof(Int64))
                            {
                                align = sizeof(Int64);
                            }

                            int align1 = (pack > sizeof(Int64)) ? sizeof(Int64) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            Int64[] v = (Int64[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new Int64[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset += sizeof(Int64))
                            {
                                v[i] = BitConverter.ToInt64(buffer, offset);
                            }
                        }
                        else if (ft.Equals(typeof(UInt64[])))
                        {
                            if (align < sizeof(UInt64))
                            {
                                align = sizeof(UInt64);
                            }

                            int align1 = (pack > sizeof(UInt64)) ? sizeof(UInt64) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            UInt64[] v = (UInt64[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = new UInt64[attr.SizeConst];
                                fi.SetValue(o, v);
                            }
                            for (int i = 0; i < attr.SizeConst; i++, offset += sizeof(UInt64))
                            {
                                v[i] = BitConverter.ToUInt64(buffer, offset);
                            }
                        }
                        else if (ft.IsClass)
                        {
                            object[] v = (object[])fi.GetValue(o);
                            if (null != v)
                            {
                                if (v.Length != attr.SizeConst)
                                {
                                    throw new Exception(t.Name + "::" + fi.Name + " not match array length !");
                                }
                            }
                            else
                            {
                                v = (object[])Activator.CreateInstance(ft, attr.SizeConst);
                                Type type = Type.GetType(ft.FullName.Replace("[]", ""));
                                for (int i = 0; i < attr.SizeConst; i++)
                                {
                                    v[i] = Activator.CreateInstance(type);
                                }
                                fi.SetValue(o, v);
                            }
                            foreach (object ob in v)
                            {
                                int size = GetAlignSize(ob.GetType());
                                if (0 != offset % size)
                                {
                                    offset = (offset / size + 1) * size;
                                }
                                if (align < size)
                                {
                                    align = size;
                                }
                                ToObject(ob.GetType(), ob, buffer, ref offset, ref alignment);
                            }
                        }
                        else
                        {
                            throw new Exception(t.Name + "::" + fi.Name + " not support typeof " + ft.Name);
                        }
                    }
                    else if (ft.IsValueType)
                    {
                        if (ft.Equals(typeof(SByte)))
                        {
                            fi.SetValue(o, (SByte)buffer[offset++]);
                        }
                        else if (ft.Equals(typeof(Byte)))
                        {
                            fi.SetValue(o, buffer[offset++]);
                        }
                        else if (ft.Equals(typeof(Int16)))
                        {
                            if (align < sizeof(Int16))
                            {
                                align = sizeof(Int16);
                            }

                            int align1 = (pack > sizeof(Int16)) ? sizeof(Int16) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            Int16 v = BitConverter.ToInt16(buffer, offset);
                            fi.SetValue(o, v);
                            offset += sizeof(Int16);
                        }
                        else if (ft.Equals(typeof(UInt16)))
                        {
                            if (align < sizeof(UInt16))
                            {
                                align = sizeof(UInt16);
                            }

                            int align1 = (pack > sizeof(UInt16)) ? sizeof(UInt16) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            UInt16 v = BitConverter.ToUInt16(buffer, offset);
                            fi.SetValue(o, v);
                            offset += sizeof(UInt16);
                        }
                        else if (ft.Equals(typeof(Int32)))
                        {
                            if (align < sizeof(Int32))
                            {
                                align = sizeof(Int32);
                            }

                            int align1 = (pack > sizeof(Int32)) ? sizeof(Int32) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            Int32 v = BitConverter.ToInt32(buffer, offset);
                            fi.SetValue(o, v);
                            offset += sizeof(Int32);
                        }
                        else if (ft.Equals(typeof(UInt32)))
                        {
                            if (align < sizeof(UInt32))
                            {
                                align = sizeof(UInt32);
                            }

                            int align1 = (pack > sizeof(UInt32)) ? sizeof(UInt32) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            UInt32 v = BitConverter.ToUInt32(buffer, offset);
                            fi.SetValue(o, v);
                            offset += sizeof(UInt32);
                        }
                        else if (ft.Equals(typeof(Int64)))
                        {
                            if (align < sizeof(Int64))
                            {
                                align = sizeof(Int64);
                            }

                            int align1 = (pack > sizeof(Int64)) ? sizeof(Int64) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            Int64 v = BitConverter.ToInt64(buffer, offset);
                            fi.SetValue(o, v);
                            offset += sizeof(Int64);
                        }
                        else if (ft.Equals(typeof(UInt64)))
                        {
                            if (align < sizeof(UInt64))
                            {
                                align = sizeof(UInt64);
                            }

                            int align1 = (pack > sizeof(UInt64)) ? sizeof(UInt64) : pack; //member alignment

                            if (0 != offset % align1)
                            {
                                offset = (offset / align1 + 1) * align1;
                            }

                            UInt64 v = BitConverter.ToUInt64(buffer, offset);
                            fi.SetValue(o, v);
                            offset += sizeof(UInt64);
                        }
                        else
                        {
                            throw new Exception(t.Name + "::" + fi.Name + " not support typeof " + ft.Name);
                        }
                    }
                    else if (ft.IsClass)
                    {
                        int size = GetAlignSize(ft);
                        if (0 != offset % size)
                        {
                            offset = (offset / size + 1) * size;
                        }
                        if (align < size)
                        {
                            align = size;
                        }
                        object sub = fi.GetValue(o);
                        if (null == sub)
                        {
                            sub = Activator.CreateInstance(ft);
                            fi.SetValue(o, sub);
                        }
                        ToObject(ft, sub, buffer, ref offset, ref alignment);
                    }
                    else
                    {
                        throw new Exception("not support typeof " + t.Name);
                    }
                }

                if (alignment < align)
                {
                    alignment = align;
                }

                int align2 = (pack > align) ? align : pack; //local alignment

                if (0 != offset % align2)
                {
                    offset = (offset / align2 + 1) * align2;
                }
            }
            catch (Exception e)
            {
                DEBUG.Exception("" + e.Message);
            }
        }
    }
}