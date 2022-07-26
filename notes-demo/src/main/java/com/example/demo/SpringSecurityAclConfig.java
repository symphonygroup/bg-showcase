package com.example.demo;


import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cache.Cache;
import org.springframework.cache.concurrent.ConcurrentMapCache;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.access.expression.method.DefaultMethodSecurityExpressionHandler;
import org.springframework.security.access.expression.method.MethodSecurityExpressionHandler;
import org.springframework.security.acls.AclPermissionCacheOptimizer;
import org.springframework.security.acls.AclPermissionEvaluator;
import org.springframework.security.acls.domain.*;
import org.springframework.security.acls.jdbc.BasicLookupStrategy;
import org.springframework.security.acls.jdbc.JdbcMutableAclService;
import org.springframework.security.acls.jdbc.LookupStrategy;
import org.springframework.security.acls.model.*;
import org.springframework.security.config.annotation.method.configuration.EnableGlobalMethodSecurity;
import org.springframework.security.config.annotation.method.configuration.GlobalMethodSecurityConfiguration;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.util.Assert;

import javax.sql.DataSource;

@Configuration
@EnableGlobalMethodSecurity(prePostEnabled = true)
public class SpringSecurityAclConfig extends GlobalMethodSecurityConfiguration {
    @Autowired
    private DataSource dataSource;

    @Override
    protected MethodSecurityExpressionHandler createExpressionHandler() {
        return buildMethodSecurityExpressionHandler();
    }

    private MethodSecurityExpressionHandler buildMethodSecurityExpressionHandler() {
        DefaultMethodSecurityExpressionHandler expressionHandler = new DefaultMethodSecurityExpressionHandler();
        AclService aclService = aclService();
        AclPermissionEvaluator permissionEvaluator = new AclPermissionEvaluator(aclService);
        expressionHandler.setPermissionEvaluator(permissionEvaluator);
        expressionHandler.setPermissionCacheOptimizer(new AclPermissionCacheOptimizer(aclService));
        return expressionHandler;
    }

    @Bean
    public AclService aclService() {
        AuditLogger auditLogger = new ConsoleAuditLogger();
        PermissionGrantingStrategy permissionGrantingStrategy = new DefaultPermissionGrantingStrategy(auditLogger);
        AclAuthorizationStrategy authStrategy = new AclAuthorizationStrategyImpl(new SimpleGrantedAuthority("ROLE_ADMIN"));
        Cache cache = new ConcurrentMapCache("aclCache");
        AclCache aclCache = new SpringCacheBasedAclCache(cache, permissionGrantingStrategy, authStrategy);
        LookupStrategy lookupStrategy = new BasicLookupStrategy(dataSource, aclCache, authStrategy, auditLogger);
        return new JdbcMutableAclService(dataSource, lookupStrategy, aclCache);
    }
}
