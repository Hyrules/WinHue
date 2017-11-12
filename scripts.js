$(function () {
  $('[data-toggle="tooltip"]').tooltip()
})

$(document).ready(function() {
  $('.image-link').magnificPopup({
    type: 'image',
    removalDelay: 500, //delay removal by X to allow out-animation
    callbacks: {
      beforeOpen: function() {
        // just a hack that adds mfp-anim class to markup 
         this.st.image.markup = this.st.image.markup.replace('mfp-figure', 'mfp-figure mfp-with-anim');
         this.st.mainClass = this.st.el.attr('data-effect');
      }
    },
  });
});

