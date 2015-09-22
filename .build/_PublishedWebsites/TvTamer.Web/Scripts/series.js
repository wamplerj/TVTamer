$(function () {

    $(document.body).on("click", ".downloadstatus", function () {

        var $this = $(this);
        var id = $this.attr("data-episodeId");
        var status = $this.text().trim();
        
        $.ajax({
            type: "POST",
            url: "/episode/updatedownloadstatus/",
            data: { id: id, status: status.toLowerCase() },
            success: function (data) {
                $this.parent().html(data);
            }
        });
    });

    $("#season").on("change", function (e) {

        var id = $("#seriesId").val();
        var season = $("option:selected", this).val();

        $.ajax({
            type: "POST",
            url: "/episode/showseasonepisodes/",
            data: { seriesId: id, season: season },
            success: function (data) {
                $("#episodes").html(data);
            }
        });
    });

});

