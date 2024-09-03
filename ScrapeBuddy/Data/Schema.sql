
CREATE TABLE IF NOT EXISTS users (
                                     id INTEGER PRIMARY KEY AUTOINCREMENT,
                                     username BLOB NOT NULL, 
                                     password BLOB NOT NULL,
                                     salt BLOB NOT NULL,
                                     IV BLOB NOT NULL,
                                     website_id INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS website_content (
                                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                                               website_name TEXT NOT NULL,
                                               url NOT NUll,
                                               clicks TEXT,
                                               x_path TEXT,
                                               element_id TEXT,
                                               content TEXT NOT NULL,
                                               login BOOL NOT NULL
);

CREATE TABLE IF NOT EXISTS notification (
                                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                                               title TEXT NOT NULL,
                                               notification TEXT NOT NULL,
                                               dismiss BOOL NOT NULL
);

