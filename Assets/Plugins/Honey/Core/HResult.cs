#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace Honey.Core
{
    public static class Hr{


        public static  TErr? EitherError<TRes1, TRes2,TRes3, TErr>(HResult<TRes1,TErr> a, HResult<TRes2,TErr> b, HResult<TRes3,TErr> c)
            where TErr:class // wouln't work correctly for structs
        {


            if (a.IsError)
            {
                return a.GetError();
            }
            if (b.IsError)
            {
                return b.GetError();
            }

            if (c.IsError)
            {
                return c.GetError();
            }
            else return null;
        }


        public static  TErr? EitherError<TRes1, TRes2, TErr>(HResult<TRes1,TErr> a, HResult<TRes2,TErr> b)
            where TErr:class // wouln't work correctly for structs
        {

            if (a.IsError)
            {
                return a.GetError();
            }
            if (b.IsError)
            {
                return b.GetError();
            }
            else return null;

        }
    }
    public readonly struct HResult<TRes,TErr>
    {
        private readonly byte tag;
        private readonly TRes? res;
        private readonly TErr? err;
        public bool IsError => tag == 1;
        public bool IsValue => tag == 0;

        private HResult(byte tag, TRes? res, TErr? err)
        {
            this.tag = tag;
            this.res = res;
            this.err = err;
        }

        public static HResult<TRes,TErr> Err(TErr err)
        {
            return new HResult<TRes, TErr>(1,default,err);
        }

        public  HResult<TRes2, TErr> Map<TRes2>(Func<TRes, TRes2> mapper)
        {
            if (!IsValue)
            {
                return HResult<TRes2, TErr>.Err(err!);
            }

            return HResult<TRes2, TErr>.Value(mapper(res!));


        }

        public static HResult<TRes, TErr> Value(TRes res)
        {
            return new HResult<TRes, TErr>(0, res, default);
        }

        public  TRes Unwrap()
        {
            if (IsError)
            {
                throw new InvalidOperationException($"tried to unwraped, error {err}");
            }

            return res;

        }
          public  TRes UnwrapOr(TRes def)
            {
                if (IsError)
                {
                    return def;
                }

                return res!;

            }

        public bool TryValue(out TRes value)
        {
            if (!IsValue)
            {
                value = default;
                return false;
            }
            value = this.res;
            return true;
        }
        public bool TryError(out TErr err)
        {
            if (!IsError)
            {
                err = default;
                return false;
            }
            err = this.err;
            return true;
        }

        public  TErr GetError()
        {
            if (IsValue)
            {
                throw new InvalidOperationException($"tried to get erorr, but it was ok");
            }

            return err;
        }


    }
}