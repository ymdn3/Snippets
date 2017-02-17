using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common.Core;
using KOILib.Common.Core.Extensions;
using Newtonsoft.Json;

namespace KOILib.Common.DataAccess
{
    public abstract class DbContextModelBase<TConnection, TModel>
        : DbContextModelBase, ICloneable
        where TConnection : DbConnection
    {
        #region Static Members
        /// <summary>
        /// テーブル名を取得します
        /// </summary>
        /// <param name="schema">指定すると、テーブル名修飾子として結果にAppendします</param>
        /// <returns></returns>
        protected static StringList GetMappedTableName(string schema = null)
        {
            return GetMappedTableName<TModel>(schema);
        }

        /// <summary>
        /// [NotMapped]を持たないプロパティ
        /// </summary>
        /// <returns></returns>
        protected static StringList GetMappedFields()
        {
            //全列
            return GetFieldsNor<TModel>(typeof(NotMappedAttribute));
        }

        /// <summary>
        /// [NotMapped][Timestamp][DatabaseGenerated]をいずれも持たないプロパティ
        /// （for INSERT対象列）
        /// </summary>
        /// <returns></returns>
        protected static StringList GetMappedFieldsWithoutTimestamp()
        {
            //INSERT対象列
            return GetFieldsNor<TModel>(typeof(NotMappedAttribute), typeof(TimestampAttribute), typeof(DatabaseGeneratedAttribute));
        }

        /// <summary>
        /// [NotMapped][Timestamp][Key][DatabaseGenerated]をいずれも持たないプロパティ
        /// （for UPDATE対象列）
        /// </summary>
        /// <returns></returns>
        protected static StringList GetMappedFieldsWithoutKey()
        {
            //UPDATE対象列
            return GetFieldsNor<TModel>(typeof(NotMappedAttribute), typeof(TimestampAttribute), typeof(DatabaseGeneratedAttribute), typeof(KeyAttribute));
        }

        /// <summary>
        /// [Key]または[Timestamp]のいずれかを持つプロパティ
        /// （for 主キーと行バージョンによる更新条件）
        /// </summary>
        /// <returns></returns>
        protected static StringList GetKeyAndTimestampFields()
        {
            //更新条件（主キーと行バージョン）
            return GetFieldsAny<TModel>(typeof(KeyAttribute), typeof(TimestampAttribute));
        }

        /// <summary>
        /// [Key]を持つプロパティ
        /// （for 主キーによる更新条件）
        /// </summary>
        /// <returns></returns>
        protected static StringList GetKeyFields()
        {
            //更新条件（主キー）
            return GetFieldsAny<TModel>(typeof(KeyAttribute));
        }

        /// <summary>
        /// [Timestamp]を持つプロパティ
        /// （for 行バージョンによる更新条件）
        /// </summary>
        /// <returns></returns>
        protected static StringList GetTimestampFields()
        {
            //更新条件（行バージョン）
            return GetFieldsAny<TModel>(typeof(TimestampAttribute));
        }

        /// <summary>
        /// 全行をSELECTするSQLを生成します
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static StringList SelectSqlFully(string schema = null)
        {
            var sql = new StringList();

            sql.Add("SELECT");

            var fields = GetMappedFields();
            sql.Add(fields.Decorate(",", "[]").ToString());

            sql.Add("FROM");

            var table = GetMappedTableName(schema);
            sql.Add(table.Decorate(".", "[]").ToString());

            return sql;
        }

        /// <summary>
        /// 全行をSELECTします
        /// </summary>
        /// <param name="db"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> SelectFullyFrom(DbContextBase db, string schema = null)
        {
            var sql = SelectSqlFully(schema).Decorate(" ").ToString();
            return db.Query<TModel>(sql);
        }

        /// <summary>
        /// WHERE句を組み立てます
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="useOr"></param>
        /// <returns></returns>
        protected static StringList BuildWhereCriteria(IEnumerable<string> fields, bool useOr = false)
        {
            var bindprefix = BindPrefix();
            var criteria = new StringList();
            foreach (var field in fields)
            {
                criteria.Add("{2}[{1}]={0}{1}", bindprefix, field, (criteria.Count > 0 ? (useOr? " OR " : " AND ") : ""));
            }
            return criteria;
        }
        /// <summary>
        /// SET句を組み立てます(UPDATE用)
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected static StringList BuildSetCriteria(IEnumerable<string> fields)
        {
            var bindprefix = BindPrefix();
            var criteria = new StringList();
            foreach (var field in fields)
            {
                criteria.Add("{2}[{1}]={0}{1}", bindprefix, field, (criteria.Count > 0 ? "," : ""));
            }
            return criteria;
        }
        /// <summary>
        /// バインドパラメータのプリフィックス文字を返します
        /// </summary>
        /// <returns></returns>
        protected static char BindPrefix()
        {
            switch (typeof(TConnection).FullName)
            {
                case "System.Data.SqlClient.SqlConnection":
                    return '@';
                default:
                    return '%';
            }
        }
        #endregion

        #region IClonable Implements
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            //JSON経由でコピー
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this), this.GetType());
        }
        #endregion

        /// <summary>
        /// 指定のプロパティがdefault値と異なる値をもつかどうか
        /// </summary>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        protected virtual bool HasField(string fieldname)
        {
            //Modelがもつ指定の名前の値を取得する
            var pi = typeof(TModel).GetProperty(fieldname);
            if (pi == null)
                return false;
            if (!pi.PropertyType.IsValueType)
                return (pi.GetValue(this) != null);
            else if (pi.PropertyType.IsNullable())
                return (pi.GetValue(this) != null); //Null許容型
            else
                return !(pi.GetValue(this).Equals(Activator.CreateInstance(pi.PropertyType))); //値型の場合
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としたSELECT SQLを生成します
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList FindSql(string schema = null)
        {
            var sql = new StringList();

            sql.Add("SELECT");

            var fields = GetMappedFields();
            sql.Add(fields.Decorate(",", "[]").ToString());

            sql.Add("FROM");

            var table = GetMappedTableName(schema);
            sql.Add(table.Decorate(".", "[]").ToString());

            //インスタンスプロパティが値を持つ場合、そのフィールドを絞り込み条件とする
            var whereFields = fields.Where(field => HasField(field));
            if (whereFields.Count() > 0)
            {
                sql.Add("WHERE");
                sql.AddRange(BuildWhereCriteria(whereFields));
            }

            return sql;
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としてSELECTし、先頭の1件を返します
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public TModel FindFirstFrom(DbContextBase db)
        {
            var sql = FindSql().Decorate(" ").ToString();
            return db.QueryFirst<TModel>(sql, this);
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としてSELECTし、全件を返します
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IEnumerable<TModel> FindFrom(DbContextBase db)
        {
            var sql = FindSql().Decorate(" ").ToString();
            return db.Query<TModel>(sql, this);
        }

        /// <summary>
        /// default値でないプロパティをソースとしたINSERT SQLを生成します。
        /// [NotMapped][Timestamp][DatabaseGenerated]属性は対象外とします。
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList InsertSql(string schema = null)
        {
            var sql = new StringList();

            sql.Add("INSERT");
            sql.Add("INTO");

            var table = GetMappedTableName(schema);
            sql.Add(table.Decorate(".", "[]").ToString());

            //インスタンスプロパティがTimestamp属性でなく、値を持つ場合、そのフィールドをINSERT対象列とする
            var fields = GetMappedFieldsWithoutTimestamp().Where(field => HasField(field));
            var valueFields = new StringList(fields);

            sql.Add("({0})", valueFields.Decorate(",", "[]").ToString());

            sql.Add("VALUES");

            sql.Add("({0})", valueFields.Decorate(",", '@', StringListQuotePositions.Pre).ToString());

            return sql;
        }

        /// <summary>
        /// default値でないプロパティをソースとしてINSERTします。
        /// [NotMapped][Timestamp][DatabaseGenerated]属性は対象外とします。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public int InsertTo(DbContextBase db)
        {
            var sql = InsertSql().Decorate(" ").ToString();
            return db.Execute(sql, this);
        }

        /// <summary>
        /// default値でないプロパティをソースとしたUPDATE SQLを生成します。
        /// [NotMapped][Timestamp][Key][DatabaseGenerated]属性はSET句の対象外、
        /// [Key][Timestamp]属性のいずれかをWHERE句の対象とします。
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList UpdateSql(string schema = null)
        {
            var sql = new StringList();

            sql.Add("UPDATE");

            var table = GetMappedTableName(schema);
            sql.Add(table.Decorate(".","[]").ToString());

            sql.Add("SET");

            //インスタンスプロパティのKeyとTimestamp属性を除くフィールドをSET対象列とする
            var setFields = GetMappedFieldsWithoutKey();
            sql.AddRange(BuildSetCriteria(setFields));

            //インスタンスプロパティがKeyまたはTimestamp属性で、値を持つ場合、そのフィールドを絞り込み条件とする
            var whereFields = GetKeyAndTimestampFields().Where(field => HasField(field));
            if (whereFields.Count() > 0)
            {
                sql.Add("WHERE");
                sql.AddRange(BuildWhereCriteria(whereFields));
            }

            return sql;
        }

        /// <summary>
        /// default値でないプロパティをソースとしてUPDATEします。
        /// [NotMapped][Timestamp][Key][DatabaseGenerated]属性はSET句の対象外、
        /// [Key][Timestamp]属性のいずれかをWHERE句の対象とします。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public int UpdateTo(DbContextBase db)
        {
            var sql = UpdateSql().Decorate(" ").ToString();
            return db.Execute(sql, this);
        }

        /// <summary>
        /// default値でないプロパティをソースとしたDELETE SQLを生成します。
        /// [Key][Timestamp]属性のいずれかをWHERE句の対象とします。
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList DeleteSql(string schema = null)
        {
            var sql = new StringList();

            sql.Add("DELETE");

            sql.Add("FROM");

            var table = GetMappedTableName(schema);
            sql.Add(table.Decorate(".","[]").ToString());

            //インスタンスプロパティがKeyまたはTimestamp属性の値を持つ場合、そのフィールドを絞り込み条件とする
            var whereFields = GetKeyAndTimestampFields().Where(field => HasField(field));
            if (whereFields.Count() > 0)
            {
                sql.Add("WHERE");
                sql.AddRange(BuildWhereCriteria(whereFields));
            }

            return sql;
        }

        /// <summary>
        /// default値でないプロパティをソースとしてDELETEします。
        /// [Key][Timestamp]属性のいずれかをWHERE句の対象とします。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public int DeleteFrom(DbContextBase db)
        {
            var sql = DeleteSql().Decorate(" ").ToString();
            return db.Execute(sql, this);
        }

    }
    public abstract class DbContextModelBase
    {
        #region Static Members
        /// <summary>
        /// マッピング対象のテーブル名を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">指定すると、テーブル名修飾子として結果にAppendします</param>
        /// <returns></returns>
        internal static StringList GetMappedTableName<T>(string schema = null)
        {
            string table = null;
            var attr = typeof(T).GetCustomAttributes<TableAttribute>();
            if (attr == null)
                table = typeof(T).Name;
            else
            {
                var aattr = attr.First();
                schema = schema ?? aattr.Schema;
                table = aattr.Name;
            }
            if (string.IsNullOrEmpty(schema))
                return new StringList(table);
            else
                return new StringList(schema, table);
        }

        /// <summary>
        /// 指定した属性型の「すべてを含まない」フィールド名を列挙します
        /// </summary>
        /// <param name="nor_attributes">属性型</param>
        /// <returns></returns>
        internal static StringList GetFieldsNor<T>(params Type[] nor_attributes)
        {
            Func<CustomAttributeData, bool> predicate = attr => nor_attributes.All(t => !attr.AttributeType.Equals(t));
            var fields = typeof(T).GetProperties()
                .Where(f => f.CustomAttributes.All(predicate))
                .Select(f => f.Name);
            return new StringList(fields);
        }

        /// <summary>
        /// 指定した属性型の「いずれかを含む」フィールド名を列挙します
        /// </summary>
        /// <param name="any_attributes">属性型</param>
        /// <returns></returns>
        internal static StringList GetFieldsAny<T>(params Type[] any_attributes)
        {
            Func<CustomAttributeData, bool> predicate = attr => any_attributes.Any(t => attr.AttributeType.Equals(t));
            var fields = typeof(T).GetProperties()
                .Where(f => f.CustomAttributes.Any(predicate))
                .Select(f => f.Name);
            return new StringList(fields);
        }

        /// <summary>
        /// すべてのフィールド名を列挙します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected internal static StringList GetFields<T>()
        {
            var fields = typeof(T).GetProperties()
                .Select(f => f.Name);
            return new StringList(fields);
        }
        #endregion

    }

}
