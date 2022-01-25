using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public class IdHelpers
    {
        public static string GenerateMalaxedGUID(int longueur = 11)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                Enumerable
                    .Range(65, 26)
                    .Select(e => ((char)e).ToString())
                    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                    .OrderBy(e => Guid.NewGuid())
                    .Take(longueur)
                    .ToList().ForEach(e => builder.Append(e));
                string id = builder.ToString();
                return id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int GenerateId(IEnumerable<int> Model, int MaxLength, out string MessageState)
        {
            try
            {
                if (Model == null || !Model.Any())
                {
                    MessageState = "Le model d'entier est null ou ne contient aucun élément. La valeur retourné par défaut est 1";
                    return 1;
                }

                for (int i = 1; i <= MaxLength; i++)
                {
                    var count = Model.Count(c => c == i);
                    if (count == 0)
                    {
                        MessageState = null;
                        return i;
                    }
                }

                MessageState = "Nous n'avons pas trouvé d'identifiant unique. La valeur par défaut retourné est 0";
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static short GenerateId(IEnumerable<short> Model, short MaxLength, out string MessageState)
        {
            try
            {
                if (Model == null || !Model.Any())
                {
                    MessageState = "Le model d'entier est null ou ne contient aucun élément. La valeur retourné par défaut est 1";
                    return 1;
                }

                for (short i = 1; i <= MaxLength; i++)
                {
                    var count = Model.Count(c => c == i);
                    if (count == 0)
                    {
                        MessageState = null;
                        return i;
                    }
                }

                MessageState = "Nous n'avons pas trouvé d'identifiant unique. La valeur par défaut retourné est 0";
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
