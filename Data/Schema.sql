-- Create the `users` table
CREATE TABLE IF NOT EXISTS users (
                                     id INTEGER PRIMARY KEY AUTOINCREMENT,
                                     username TEXT,
                                     password TEXT,
                                     website_id INTEGER
);

-- Create the `website_content` table
CREATE TABLE IF NOT EXISTS website_content (
                                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                                               website_name TEXT,
                                               x_path TEXT,
                                               element_id TEXT,
                                               content TEXT
);
