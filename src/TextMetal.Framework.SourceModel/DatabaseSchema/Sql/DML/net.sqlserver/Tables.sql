/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- tables[schema]
select
	cast(null as int) as ObjectId,
	sys_s.name as SchemaName,
	sys_t.name as TableName,
	cast(0 as bit) as IsView,

	(select
		sys2_kc.name
	from
		sys.key_constraints sys2_kc
		inner join sys.tables sys2_t on sys2_t.object_id = sys2_kc.parent_object_id
		inner join sys.schemas sys2_s on sys2_s.schema_id = sys2_t.schema_id
	where
		sys2_kc.type = 'PK'		
		and sys2_s.name = sys_s.name
		and sys2_t.name = sys_t.name
	) as PrimaryKeyName
from
    sys.tables sys_t
	inner join sys.schemas sys_s on sys_s.schema_id = sys_t.schema_id
where
	sys_s.name = @SchemaName

union all

select
	cast(null as int) as ObjectId,
	sys_s.name as SchemaName,
    sys_v.name as TableName,
	cast(1 as bit) as IsView,
	null as PrimaryKeyName
from
    sys.views sys_v
	inner join sys.schemas sys_s on sys_s.schema_id = sys_v.schema_id
where
	sys_s.name = @SchemaName
