using System.IO;

namespace IronSnappy
{
   class Program
   {
      static void Main(string[] args)
      {
         using(FileStream dest = File.Create("c:\\tmp\\sawyer.txt"))
         {

            using(Stream w = Snappy.OpenWriter(dest))
            {
               using(FileStream src = File.OpenRead("TestData/Mark.Twain-Tom.Sawyer.txt"))
               {
                  src.CopyTo(w);
               }
            }
         }
      }
   }
}