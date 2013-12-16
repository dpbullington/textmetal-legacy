/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- triggers[schema, table|view]
select
	sys_tr.object_id as ObjectId,
	sys_s.name as SchemaName,
	sys_o.name as TableName,
	sys_tr.name as TriggerName,
	cast(case
		when sys_tr.type = 'TA' then 1
		else 0
	end as bit) as IsClrTrigger,
	sys_tr.is_disabled as IsTriggerDisabled,
	sys_tr.is_not_for_replication as IsTriggerNotForReplication,
	sys_tr.is_instead_of_trigger as IsInsteadOfTrigger
from
	sys.triggers sys_tr
	inner join sys.objects sys_o on sys_o.object_id = sys_tr.parent_id
	inner join sys.schemas sys_s on sys_s.schema_id = sys_o.schema_id
where
	sys_o.type in ('U', 'V')
	and sys_tr.parent_class = 1
	and sys_s.name = @SchemaName
	and sys_o.name = @TableOrViewName
order by
	sys_tr.name asc