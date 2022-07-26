package com.example.demo;

import org.springframework.security.acls.domain.AuditLogger;
import org.springframework.security.acls.model.AccessControlEntry;
import org.springframework.util.Assert;

public class ConsoleLogger implements AuditLogger {

    @Override
    public void logIfNeeded(boolean granted, AccessControlEntry ace) {
        Assert.notNull(ace, "AccessControlEntry required");
        if (granted) {
            System.out.println("GRANTED due to ACE: " + ace);
        } else {
            System.out.println("DENIED due to ACE: " + ace);
        }
    }
}
