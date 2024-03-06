<cfparam name="attributes.data" default="">

<table class="table table-striped table-hover">
    <thead>
        <tr>
            <th scope="col">Company</th>
            <th scope="col">Job Title</th>
            <th scope="col">Apply Link</th>
            <th scope="col">Applied?</th>
        </tr>
    </thead>
    <tbody>
        <cfoutput query="attributes.data">
            <tr>
                <th scope="row">#attributes.data.companyName#</th>
                <td style="width: auto">#attributes.data.postTitle#</td>
                <td>
                    <cfif FindNoCase("indeed.com", attributes.data.link)>
                        <a href="#attributes.data.link#" class="btn btn-link">Indeed.com</a>
                    <cfelseif FindNoCase("dice.com", attributes.data.link)>
                        <a href="#attributes.data.link#" class="btn btn-link">Dice.com</a>
                    <cfelse>
                        <a href="#attributes.data.link#" class="btn btn-link">Link</a>
                    </cfif>
                <td scope="row">
                     <button class="btn toggle-applied #attributes.data.hasApplied eq 0 ? 'btn-secondary' : 'btn-success'# btn-sm" 
                    data-applied="#attributes.data.hasApplied#" 
                    data-id="#attributes.data.id#" 
                    onclick="toggleApplicationStatus(this);" 
                    style="width: 100px;"> <!-- Fixed width for consistency -->
                #attributes.data.hasApplied eq 0 ? 'Not Applied' : 'Applied'#
            </button>
                </td>
            </td>
        </tr>
    </cfoutput>
    </tbody>
</table>
<script>
$(document).ready(function() {
    $('.toggle-applied').click(function() {
        var $this = $(this);
        var jobId = $this.data('job-id');
        var hasApplied = $this.data('applied');

        if (hasApplied == 0) {
            // Update the database via AJAX
            $.post('/path/to/updateApplicationStatus.cfm', { jobId: jobId, hasApplied: 1 }, function(response) {
                // Check response to ensure the update was successful
                if (response.success) {
                    $this.addClass('btn-success').text('Applied');
                    $this.data('applied', 1); // Update the applied status
                } else {
                    // Handle error
                    alert('There was a problem updating the application status.');
                }
            });
        }
    });
});
</script>
