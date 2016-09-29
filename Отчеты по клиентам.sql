select * from ord order by id desc
select sum(SummOrder) from ORD where subjectId>100064 and state<>'D'
select INN,Name,isnull((select sum(SummOrder) from ORD where subjectId=subj.id and ord.id>101453 and state<>'D'),0) as SummOrder from SUBJ where id>100064

select * from (select INN,Name,isnull((select sum(SummOrder) from ORD where subjectId=subj.id and ord.id>101453 and state<>'D'),0) as SummOrder from SUBJ) as orders where SummOrder>0