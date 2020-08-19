$( "#sidebar" ).before( "<div class='menuTrigger'></div>" );

$(this).show;
$('.menuTrigger').click(function() {
              $(this).toggleClass('selected');
              $('#sidebar').slideToggle( "slow", function() {
  
  });

});