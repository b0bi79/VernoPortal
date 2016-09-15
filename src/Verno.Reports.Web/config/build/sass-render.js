var fs = require('fs');
var sass = require('node-sass');
sass.render({
    file: helpers.root('/src/app/theme') + '/login.scss',
    outFile: helpers.root('/wwwroot') + '/login.css'
}, function (error, result) {
    if (!error) {
        console.log('sass compiled ok');
        fs.writeFile(helpers.root('/wwwroot') + '/login.css', result.css, function (err) {
            if (err) console.log('login.css write error: ' + err);
        });
    }
    else console.log('sass error: ' + error);
});
