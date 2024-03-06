// JobsTable.cfc
component {

    // Function to generate HTML table from a query
    public string function generateTable(required query getJobs) output="false" {
        var tableHTML = "<table class='table table-striped table-hover'>";
        tableHTML &= "<thead><tr><th scope='col'>id</th><th scope='col'>Job</th><th scope='col'>Link</th></tr></thead><tbody>";

        for (var i = 1; i <= getJobs.recordCount; i++) {
            tableHTML &= "<tr>";
            tableHTML &= "<th scope='row'>" & getJobs.id[i] & "</th>";
            tableHTML &= "<td style='width: auto'>" & getJobs.postTitle[i] & "</td>";
            tableHTML &= "<td>";

            if (FindNoCase("indeed.com", getJobs.link[i])) {
                tableHTML &= "<a href='" & getJobs.link[i] & "' class='btn btn-link'>Indeed.com</a>";
            } else if (FindNoCase("dice.com", getJobs.link[i])) {
                tableHTML &= "<a href='" & getJobs.link[i] & "' class='btn btn-link'>Dice.com</a>";
            } else {
                tableHTML &= "<a href='" & getJobs.link[i] & "' class='btn btn-link'>Link</a>";
            }

            tableHTML &= "</td></tr>";
        }

        tableHTML &= "</tbody></table>";

        return tableHTML;
    }

}
