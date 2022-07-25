# Model生成器

1. DBHelper配套的Model生成器

2. 如要支持EntityFramework和EntityFrameworkCore，请修改AttributeStrings类和模板

## 支持的数据库

1. Oracle
2. MSSQL
3. MySQL
4. PostgreSQL
5. SQLite

## 注意事项

如果报错并提示"Model生成器未实现数据库字段类型xxx的转换"，请修改对应数据库的Dal类的ConvertDataType方法



