/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- triggers
select
	sys_tr.object_id as ObjectId,
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
where
	sys_tr.parent_class = 0
order by
	sys_tr.name asc