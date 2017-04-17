using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common;
using KOILib.Common.Extensions;
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
        public static IEnumerable<string> GetMappedTableName(string schema = null)
        {
            return GetMappedTableName<TModel>(schema);
        }

        /// <summary>
        /// [NotMapped]を持たないプロパティ
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetMappedFields()
        {
            //全列
            return GetFieldsNor<TModel>(typeof(NotMappedAttribute));
        }

        /// <summary>
        /// [NotMapped][Timestamp][DatabaseGenerated]をいずれも持たないプロパティ
        /// （for INSERT対象列）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetMappedFieldsWithoutTimestamp()
        {
            //INSERT対象列
            return GetFieldsNor<TModel>(typeof(NotMappedAttribute), typeof(TimestampAttribute), typeof(DatabaseGeneratedAttribute));
        }

        /// <summary>
        /// [NotMapped][Timestamp][Key][DatabaseGenerated]をいずれも持たないプロパティ
        /// （for UPDATE対象列）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetMappedFieldsWithoutKey()
        {
            //UPDATE対象列
            return GetFieldsNor<TModel>(typeof(NotMappedAttribute), typeof(TimestampAttribute), typeof(DatabaseGeneratedAttribute), typeof(KeyAttribute));
        }

        /// <summary>
        /// [Key]または[Timestamp]のいずれかを持つプロパティ
        /// （for 主キーと行バージョンによる更新条件）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetKeyAndTimestampFields()
        {
            //更新条件（主キーと行バージョン）
            return GetFieldsAny<TModel>(typeof(KeyAttribute), typeof(TimestampAttribute));
        }

        /// <summary>
        /// [Key]を持つプロパティ
        /// （for 主キーによる更新条件）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetKeyFields()
        {
            //更新条件（主キー）
            return GetFieldsAny<TModel>(typeof(KeyAttribute));
        }

        /// <summary>
        /// [Timestamp]を持つプロパティ
        /// （for 行バージョンによる更新条件）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetTimestampFields()
        {
            //更新条件（行バージョン）
            return GetFieldsAny<TModel>(typeof(TimestampAttribute));
        }

        /// <summary>
        /// すべてのフィールド名を列挙します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<string> GetFields()
        {
            return GetFields<TModel>();
        }

        /// <summary>
        /// 全行をSELECTするSQLを生成します
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static StringList SelectSqlFully(IEnumerable<string> orders = null, string schema = null)
        {
            var fields = GetMappedFields();
            var sql = SelectSql<TModel>(fields, null, orders, schema);
            return sql;
        }

        /// <summary>
        /// 全行をSELECTします
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orders"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> SelectFullyFrom(DbContextBase db, IEnumerable<string> orders = null, string schema = null)
        {
            var sql = SelectSqlFully(orders, schema);
            return db.Query<TModel>(sql.ToDecorateString(" "));
        }

        /// <summary>
        /// バインドパラメータとイコール記号の式（FIELDNAME=%FIELDNAME）を組み立てます
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected static IEnumerable<string> BuildEqExpressions(IEnumerable<string> fields)
        {
            return BuildEqExpressions<TConnection>(fields);
        }

        /// <summary>
        /// バインドパラメータのプリフィックス文字を返します
        /// </summary>
        /// <returns></returns>
        protected static char BindPrefix()
        {
            return BindPrefix<TConnection>();
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

        protected string[] _likeFields;
        /// <summary>
        /// 生成されるWHERE句で、LIKE演算子が使用されるフィールドを指定します。
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public DbContextModelBase<TConnection, TModel> LikeExpr(params string[] fields)
        {
            _likeFields = fields;
            return this;
        }

        protected string[] _orderFields;
        /// <summary>
        /// FindSql()メソッドが生成するSQLに、ORDER BY句が指定されるよう設定します。
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public DbContextModelBase<TConnection, TModel> OrderBy(params string[] fields)
        {
            _orderFields = fields;
            return this;
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としたSELECT SQLを生成します
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList FindSql(IEnumerable<string> orders = null, string schema = null)
        {
            var fields = GetMappedFields();

            //インスタンスプロパティが値を持つ場合、そのフィールドを絞り込み条件とする
            var whereFields = fields.Where(x => HasValue(x));
            var whereCriteria = BuildWhereCriteria(whereFields, useOR: false);

            var sql = SelectSql<TModel>(fields, whereCriteria, orders ?? _orderFields, schema);
            return sql;
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としてSELECTし、全件を返します
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orders">ORDER BYに指定する項目</param>
        /// <returns></returns>
        public IEnumerable<TModel> FindFrom(DbContextBase db, IEnumerable<string> orders)
        {
            var sql = FindSql(orders: orders);
            return db.Query<TModel>(sql.ToDecorateString(" "), this);
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としてSELECTし、全件を返します
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IEnumerable<TModel> FindFrom(DbContextBase db)
        {
            var sql = FindSql();
            return db.Query<TModel>(sql.ToDecorateString(" "), this);
        }

        /// <summary>
        /// default値でないプロパティをWHERE条件としてSELECTし、先頭の1件を返します
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public TModel FindFirstFrom(DbContextBase db)
        {
            var sql = FindSql();
            return db.QueryFirst<TModel>(sql.ToDecorateString(" "), this);
        }

        /// <summary>
        /// default値でないプロパティをソースとしたINSERT SQLを生成します。
        /// [NotMapped][Timestamp][DatabaseGenerated]属性は対象外とします。
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList InsertSql(string schema = null)
        {
            //インスタンスプロパティがTimestamp属性でなく、値を持つ場合、そのフィールドをINSERT対象列とする
            var fields = GetMappedFieldsWithoutTimestamp().Where(x => HasValue(x));

            //バインドパラメータのプリフィックスを付ける
            var prefix = BindPrefix();
            var values = fields.Select(x => prefix + x);

            var sql = InsertSql<TModel>(fields, values, schema);
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
            var sql = InsertSql();
            return db.Execute(sql.ToDecorateString(" "), this);
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
            //インスタンスプロパティのKeyとTimestamp属性を除くフィールドをSET対象列とする
            var setFields = GetMappedFieldsWithoutKey();
            var setStatement = BuildSetStatement(setFields);

            //インスタンスプロパティがKeyまたはTimestamp属性で、値を持つ場合、そのフィールドを絞り込み条件とする
            var whereFields = GetKeyAndTimestampFields().Where(x => HasValue(x));
            var whereCriteria = BuildWhereCriteria(whereFields, useOR: false);

            var sql = UpdateSql<TModel>(setStatement, whereCriteria, schema);
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
            var sql = UpdateSql();
            return db.Execute(sql.ToDecorateString(" "), this);
        }

        /// <summary>
        /// default値でないプロパティをソースとしたDELETE SQLを生成します。
        /// [Key][Timestamp]属性のいずれかをWHERE句の対象とします。
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual StringList DeleteSql(string schema = null)
        {
            //インスタンスプロパティがKeyまたはTimestamp属性の値を持つ場合、そのフィールドを絞り込み条件とする
            var whereFields = GetKeyAndTimestampFields().Where(x => HasValue(x));
            var whereCriteria = BuildWhereCriteria(whereFields, useOR: false);

            var sql = DeleteSql<TModel>(whereCriteria, schema);
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
            var sql = DeleteSql();
            return db.Execute(sql.ToDecorateString(" "), this);
        }

        /// <summary>
        /// バインドパラメータと指定の演算子の式(FIELDNAME OPE %FIELDNAME)を組み立てます
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public virtual string BuildExpression(string field, string @operator)
        {
            var prefix = BindPrefix();
            return BuildExpression(field, @operator, prefix);
        }

        /// <summary>
        /// WHERE句のための式を組み立てます
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> BuildWhereExpressions(IEnumerable<string> fields)
        {
            return fields.Select(x =>
            {
                //演算子選択
                var ope = (_likeFields != null && _likeFields.Contains(x)) ? "LIKE" : "=";
                return BuildExpression(x, ope);
            });
        }

        /// <summary>
        /// WHERE句を組み立てます
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="useOR"></param>
        /// <returns></returns>
        public virtual string BuildWhereCriteria(IEnumerable<string> fields, bool useOR)
        {
            var expressions = BuildWhereExpressions(fields);
            return string.Join(useOR ? " OR " : " AND ", expressions);
        }

        /// <summary>
        /// SET句を組み立てます(UPDATE用)
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual String BuildSetStatement(IEnumerable<string> fields)
        {
            return BuildSetStatement<TConnection>(fields);
        }

        /// <summary>
        /// 指定のプロパティが、default値と異なる値をもつかどうか
        /// </summary>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        public bool HasValue(string fieldname)
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

    }

    public abstract class DbContextModelBase
    {
        #region Static Members
        /// <summary>
        /// 標準的なSELECT SQLを生成します。
        /// </summary>
        /// <param name="selectStatement"></param>
        /// <param name="fromStatement"></param>
        /// <param name="whereCriteria"></param>
        /// <param name="orderbyStatement"></param>
        /// <returns></returns>
        public static StringList SelectSql(string selectStatement, string fromStatement, string whereCriteria, string orderbyStatement)
        {
            var sql = new StringList()
                .Add("SELECT")
                .Add(selectStatement)
                .Add("FROM")
                .Add(fromStatement);

            if (!string.IsNullOrEmpty(whereCriteria))
            {
                if (!whereCriteria.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    sql.Add("WHERE");
                sql.Add(whereCriteria);
            }

            if (!string.IsNullOrEmpty(orderbyStatement))
                sql.Add("ORDER BY")
                    .Add(orderbyStatement);

            return sql;
        }
        /// <summary>
        /// 標準的なSELECT SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="selectStatement"></param>
        /// <param name="whereCriteria"></param>
        /// <param name="orderbyStatement"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList SelectSql<T>(string selectStatement, string whereCriteria, string orderbyStatement, string schema = null)
        {
            var table = GetMappedTableName<T>(schema);
            var fromStatement = string.Join(".", table);
            return SelectSql(selectStatement, fromStatement, whereCriteria, orderbyStatement);
        }
        /// <summary>
        /// 標準的なSELECT SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="selects"></param>
        /// <param name="whereCriteria"></param>
        /// <param name="orders"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList SelectSql<T>(IEnumerable<string> selects, string whereCriteria, IEnumerable<string> orders, string schema = null)
        {
            var selectStatement = string.Join(", ", selects);
            var orderbyStatement = (orders == null || orders.Count() == 0) ? null : string.Join(", ", orders);
            return SelectSql<T>(selectStatement, whereCriteria, orderbyStatement, schema);
        }

        /// <summary>
        /// 標準的なINSERT SQLを生成します。
        /// </summary>
        /// <param name="intoStatement"></param>
        /// <param name="fieldsStatement"></param>
        /// <param name="valuesStatement"></param>
        /// <returns></returns>
        public static StringList InsertSql(string intoStatement, string fieldsStatement, string valuesStatement)
        {
            var sql = new StringList()
                .Add("INSERT")
                .Add("INTO")
                .Add(intoStatement)
                .Add("({0})", fieldsStatement)
                .Add("VALUES")
                .Add("({0})", valuesStatement);

            return sql;
        }
        /// <summary>
        /// 標準的なINSERT SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="fieldsStatement"></param>
        /// <param name="valuesStatement"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList InsertSql<T>(string fieldsStatement, string valuesStatement, string schema = null)
        {
            var table = GetMappedTableName<T>(schema);
            var intoStatement = string.Join(".", table);
            return InsertSql(intoStatement, fieldsStatement, valuesStatement);
        }
        /// <summary>
        /// 標準的なINSERT SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="intoFields"></param>
        /// <param name="values"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList InsertSql<T>(IEnumerable<string> fields, IEnumerable<string> values, string schema = null)
        {
            var fieldsStatement = string.Join(", ", fields);
            var valuesStatement = string.Join(", ", values);
            return InsertSql<T>(fieldsStatement, valuesStatement, schema);
        }

        /// <summary>
        /// 標準的なUPDATE SQLを生成します。
        /// </summary>
        /// <param name="targetStatement"></param>
        /// <param name="setStatement"></param>
        /// <param name="whereCriteria"></param>
        /// <returns></returns>
        public static StringList UpdateSql(string targetStatement, string setStatement, string whereCriteria)
        {
            var sql = new StringList()
                .Add("UPDATE")
                .Add(targetStatement)
                .Add("SET")
                .Add(setStatement);

            if (!string.IsNullOrEmpty(whereCriteria))
            {
                if (!whereCriteria.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    sql.Add("WHERE");
                sql.Add(whereCriteria);
            }

            return sql;
        }
        /// <summary>
        /// 標準的なUPDATE SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="setStatement"></param>
        /// <param name="whereCriteria"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList UpdateSql<T>(string setStatement, string whereCriteria, string schema = null)
        {
            var table = GetMappedTableName<T>(schema);
            var targetStatement = string.Join(".", table);
            return UpdateSql(targetStatement, setStatement, whereCriteria);
        }
        /// <summary>
        /// 標準的なUPDATE SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="setExpressions"></param>
        /// <param name="whereCriteria"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList UpdateSql<T>(IEnumerable<string> setExpressions, string whereCriteria, string schema = null)
        {
            var setStatement = string.Join(", ", setExpressions);
            return UpdateSql<T>(setStatement, whereCriteria, schema);
        }

        /// <summary>
        /// 標準的なDELETE SQLを生成します。
        /// </summary>
        /// <param name="fromStatement"></param>
        /// <param name="whereCriteria"></param>
        /// <returns></returns>
        public static StringList DeleteSql(string fromStatement, string whereCriteria)
        {
            var sql = new StringList()
                .Add("DELETE")
                .Add("FROM")
                .Add(fromStatement);

            if (!string.IsNullOrEmpty(whereCriteria))
            {
                if (!whereCriteria.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    sql.Add("WHERE");
                sql.Add(whereCriteria);
            }

            return sql;
        }
        /// <summary>
        /// 標準的なDELETE SQLを生成します。
        /// </summary>
        /// <typeparam name="T">モデル</typeparam>
        /// <param name="whereCriteria"></param>
        /// <param name="schema">テーブルスキーマ</param>
        /// <returns></returns>
        public static StringList DeleteSql<T>(string whereCriteria, string schema = null)
        {
            var table = GetMappedTableName<T>(schema);
            var fromStatement = string.Join(".", table);
            return DeleteSql(fromStatement, whereCriteria);
        }

        /// <summary>
        /// バインドパラメータと指定の演算子の式(FIELDNAME OPE %FIELDNAME)を組み立てます
        /// </summary>
        /// <param name="field"></param>
        /// <param name="bindprefix"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        protected static string BuildExpression(string field, string @operator, char bindprefix)
        {
            return string.Format("{2} {0} {1}{2}", @operator, bindprefix, field);
        }

        /// <summary>
        /// バインドパラメータとイコール記号の式（FIELDNAME=%FIELDNAME）を組み立てます
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected static IEnumerable<string> BuildEqExpressions<T>(IEnumerable<string> fields)
            where T : DbConnection
        {
            var @operator = "=";
            var bindprefix = BindPrefix<T>();
            var expressions = fields.Select(x => BuildExpression(x, @operator, bindprefix));
            return expressions;
        }

        /// <summary>
        /// バインドパラメータとLIKE演算子の式（FIELDNAME LIKE %FIELDNAME）を組み立てます
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected static IEnumerable<string> BuildLikeExpressions<T>(IEnumerable<string> fields)
            where T : DbConnection
        {
            var @operator = "LIKE";
            var bindprefix = BindPrefix<T>();
            var expressions = fields.Select(x => BuildExpression(x, @operator, bindprefix));
            return expressions;
        }

        /// <summary>
        /// WHERE句を組み立てます
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="useOR"></param>
        /// <returns></returns>
        [Obsolete("LIKE演算子の式考慮のため削除されました", true)]
        protected static string BuildWhereCriteria<T>(IEnumerable<string> fields, bool useOR)
            where T : DbConnection
        {
            return string.Join(useOR ? " OR " : " AND ", BuildEqExpressions<T>(fields));
        }

        /// <summary>
        /// SET句を組み立てます（UPDATE用）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected static string BuildSetStatement<T>(IEnumerable<string> fields)
            where T : DbConnection
        {
            return string.Join(", ", BuildEqExpressions<T>(fields));
        }

        /// <summary>
        /// バインドパラメータのプリフィックス文字を返します
        /// </summary>
        /// <returns></returns>
        protected static char BindPrefix<T>()
            where T : DbConnection
        {
            switch (typeof(T).FullName)
            {
                case "System.Data.SqlClient.SqlConnection":
                    return '@';
                default:
                    return '%';
            }
        }

        /// <summary>
        /// マッピング対象のテーブル名を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">指定すると、テーブル名修飾子として結果にAppendします</param>
        /// <returns></returns>
        protected static IEnumerable<string> GetMappedTableName<T>(string schema = null)
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
                return new string[] { table };
            else
                return new string[] { schema, table };
        }

        /// <summary>
        /// 指定した属性型の「すべてを含まない」フィールド名を列挙します
        /// </summary>
        /// <param name="nor_attributes">属性型</param>
        /// <returns></returns>
        protected static IEnumerable<string> GetFieldsNor<T>(params Type[] nor_attributes)
        {
            Func<CustomAttributeData, bool> predicate = attr => nor_attributes.All(x => !attr.AttributeType.Equals(x));
            var fields = typeof(T).GetProperties()
                .Where(x => x.CustomAttributes.All(predicate))
                .Select(x => x.Name);
            return fields;
        }

        /// <summary>
        /// 指定した属性型の「いずれかを含む」フィールド名を列挙します
        /// </summary>
        /// <param name="any_attributes">属性型</param>
        /// <returns></returns>
        protected static IEnumerable<string> GetFieldsAny<T>(params Type[] any_attributes)
        {
            Func<CustomAttributeData, bool> predicate = attr => any_attributes.Any(x => attr.AttributeType.Equals(x));
            var fields = typeof(T).GetProperties()
                .Where(x => x.CustomAttributes.Any(predicate))
                .Select(x => x.Name);
            return fields;
        }

        /// <summary>
        /// すべてのフィールド名を列挙します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static IEnumerable<string> GetFields<T>()
        {
            var fields = typeof(T).GetProperties()
                .Select(x => x.Name);
            return fields;
        }
        #endregion

    }

}
