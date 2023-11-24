CREATE TABLE motion_detector (
	id SERIAL PRIMARY KEY,
	name VARCHAR NOT NULL,
	secret_key VARCHAR NOT NULL,
	last_motion TIMESTAMP
);