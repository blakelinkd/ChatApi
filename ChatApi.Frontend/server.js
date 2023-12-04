// server.js
const express = require('express');
const path = require('path');
const morgan = require('morgan');
const cors = require('cors');


const app = express();
app.use(cors());


// Add morgan middleware for logging
app.use(morgan('combined'));

// Serve static files from the dist directory
app.use(express.static(__dirname + '/dist/chat-api.frontend/browser'));

// Send all requests to index.html
app.get('/*', function(req, res) {
    res.sendFile(path.join(__dirname + '/dist/chat-api.frontend/browser/index.html'));
});

// Error handling middleware
app.use(function(err, req, res, next) {
    console.error(err.stack);
    res.status(500).send('Something broke!');
});

// Default Heroku port
app.listen(process.env.PORT || 4200, () => {
    console.log(`Server is running on port ${process.env.PORT || 4200}`);
});