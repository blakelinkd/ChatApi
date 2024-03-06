<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <title>List of Jobs</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">

</head>
<body>
    <cfquery datasource="jobs" name="getJobs">
        SELECT * FROM jobs
        ORDER BY ID DESC
        LIMIT 50
    </cfquery>


  <cf_JobsTable data="#getJobs#"/>


    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</body>
</html>
