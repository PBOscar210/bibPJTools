using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibPJTools
{
    public static class sqlScript
    {
        public static string sqlIdTableByObjet(string dbType, long IdObjet, Dictionary<string, object> lstParam)
        {
            lstParam.Add("@P_IdObjet", IdObjet);

            string sql;
            sql = "SELECT T.TableId";
            sql += '\n' + "  FROM {0}.GEN_Objet O  WITH (NOLOCK)";
            sql += '\n' + "  JOIN {0}.SYS_Tables T ON T.TableId = IdTableSource";
            sql += '\n' + " WHERE O.IdObjet = @P_IdObjet";
            sql = string.Format(sql, dbType);

            return string.Format(sql, dbType);
        }

        public static string sqlIdUser(string dbType, string CodeUsager, Dictionary<string, object> lstParam)
        {
            lstParam.Add("@P_CodeUsager", CodeUsager);

            string sql;
            sql = "SELECT A.IdEntiteUsager";
            sql += '\n' + "  FROM {0}.SYS_Usager A";
            sql += '\n' + " WHERE A.CodeUsager = @P_CodeUsager";
            sql = string.Format(sql, dbType);

            return string.Format(sql, dbType);
        }


        public static string sqlTableList(string dbType)
        {
            string sql;
            sql = "SELECT A.TableId, COALESCE(A.TableName, A.TableDescription) TabDesc";
            sql += '\n' + "  FROM {0}.sys_tables A";
            sql += '\n' + " WHERE EXISTS (SELECT * FROM {0}.GEN_Objet  WITH (NOLOCK) WHERE IdTableSource = A.TableId)";
            sql += '\n' + " ORDER BY 2";

            return string.Format(sql, dbType);
        }

        public static string sqlObjetList(string dbType, int nbSqlTop, long IdObjet, long IdTable, string dsObjetFindText, Dictionary<string, object> lstParam)
        {
            lstParam.Add("@P_IdTableSource", IdTable);
            lstParam.Add("@P_nbSqlTop", nbSqlTop);
            lstParam.Add("@P_IdObjet", IdObjet);

            string selTop = "";
            if (nbSqlTop > 0)
                selTop += "TOP (@P_nbSqlTop) ";

            string sql;
            sql = "SELECT * FROM (";
            sql += '\n' + "  SELECT " + selTop + "A.IdObjet, A.Code CodeObjet, COALESCE(NULLIF(A.Description, ''), A.Code) DescObjet, B.Nom_Division NomDiv, A.IdTableSource";
            sql += '\n' + "    FROM {0}.GEN_Objet A";
            sql += '\n' + "    LEFT JOIN {0}.SYS_Divisions B ON B.IdDivision = A.IdDiv";
            sql += '\n' + "   WHERE IdTableSource = @P_IdTableSource";
            sql += '\n' + "     AND (NULLIF(A.Code, '') IS NOT NULL OR NULLIF(A.Description, '') IS NOT NULL)";
            if (dsObjetFindText != "")
            {
                List<string> lstLike = dsObjetFindText.Split(new char[] { ' ' }).ToList();
                for (int I = 0; I < lstLike.Count; I++)
                {
                    string lSql = "    AND (";
                    if (I > 0)
                        lSql = "     OR ";

                    lSql += '\n' + "A.Code LIKE @P_FindText" + I.ToString() + " OR A.Description LIKE @P_FindText" + I.ToString();

                    if (I == lstLike.Count - 1)
                        lSql += '\n' + ")";

                    sql += lSql;
                    lstParam.Add("@P_FindText" + I.ToString(), "%" + lstLike[I] + "%");
                }
            }
            sql += '\n' + "   ORDER BY 3) A";
            if (IdObjet > 0)
            {
                sql += '\n' + "UNION";
                sql += '\n' + "SELECT A.IdObjet, A.Code CodeObjet, COALESCE(NULLIF(A.Description, ''), A.Code) DescObjet, B.Nom_Division NomDiv, A.IdTableSource";
                sql += '\n' + "  FROM {0}.GEN_Objet A";
                sql += '\n' + "  LEFT JOIN {0}.SYS_Divisions B ON B.IdDivision = A.IdDiv";
                sql += '\n' + " WHERE IdTableSource = @P_IdTableSource";
                sql += '\n' + "   AND IdObjet = @P_IdObjet";
                sql += '\n' + "   AND (NULLIF(A.Code, '') IS NOT NULL OR NULLIF(A.Description, '') IS NOT NULL)";
                sql += '\n' + " ORDER BY 3";
            }

            return string.Format(sql, dbType);
        }

        public static string sqlGDocDirByObjet(string dbType, long IdObjet, long IdUser, Dictionary<string, object> lstParam)
        {
            lstParam.Add("@P_IdObjet", IdObjet);
            lstParam.Add("@P_IdUser", ValTools.valOrNull(IdUser));

            string sql = "{0}.SP_GEN_GetGDocDirByObjet @P_IdObjet, @P_IdUser";
            return string.Format(sql, dbType);
        }

        public static string sqlAssignFile(string dbType, long IdObjet, Guid parentStreamId, long? nIdDir, Byte[] fs, string fileName, long IdUser, string desc, string descDet, Dictionary<string, object> lstParam)
        {
            lstParam.Add("@IdObjet", IdObjet);
            lstParam.Add("@stream_id", Guid.NewGuid());
            lstParam.Add("@parent_stream_id", bibPJTools.ValTools.valOrNull(parentStreamId));
            lstParam.Add("@IdDocDir", nIdDir);
            lstParam.Add("@NomFichier", fileName);
            lstParam.Add("@DescFile", bibPJTools.ValTools.valOrNull(desc));
            lstParam.Add("@DescDetFile", bibPJTools.ValTools.valOrNull(descDet));
            lstParam.Add("@IdEntiteUser", bibPJTools.ValTools.valOrNull(IdUser));
            lstParam.Add("@IsDirectory", false);
            lstParam.Add("@IsDirectoryTpl", false);
            lstParam.Add("@FileData", fs);

            string sql = "{0}.GEN_SP_Attachments_AddFile";
            return string.Format(sql, dbType);
        }
    }
}
