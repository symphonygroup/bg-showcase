package com.example.demo.memo;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.access.prepost.PostFilter;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.acls.domain.*;
import org.springframework.security.acls.model.*;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import javax.transaction.Transactional;
import java.util.ArrayList;
import java.util.List;
import java.util.ListIterator;
import java.util.stream.Collectors;

@Transactional
@RestController
@RequestMapping("/memos")
public class MemoController {
    @Autowired
    private Memos memos;

    @Autowired
    private MutableAclService aclService;

    @RequestMapping(value = "/", method = RequestMethod.GET)
    @PostFilter("hasPermission(filterObject, read) or hasPermission(filterObject, admin)")
    public List<Memo> list() throws Exception {
        return memos.findAll();
    }

    @RequestMapping(value = "/", method = RequestMethod.POST)
    public Memo create(@AuthenticationPrincipal(expression = "username") String username, @RequestBody String note) throws Exception {
        Memo m = new Memo();
        m.owner = username;
        m.data = note;
        Memo e = memos.saveAndFlush(m);

        addPermission(username, e.id, BasePermission.ADMINISTRATION);
        return e;
    }

    @RequestMapping(value = "/{id}", method = RequestMethod.GET)
    @PreAuthorize("hasPermission(#id, 'com.example.demo.memo.Memo', read) or hasPermission(#id, 'com.example.demo.memo.Memo', admin)")
    public Memo retrieve(@PathVariable Long id) throws Exception {
        return memos.findById(id).orElseThrow();
    }

    @RequestMapping(value = "/{id}", method = RequestMethod.POST)
    @PreAuthorize("hasPermission(#id, 'com.example.demo.memo.Memo', write) or hasPermission(#id, 'com.example.demo.memo.Memo', admin)")
    public Memo update(@PathVariable Long id, @RequestBody Memo memo) throws Exception {
        Memo db = memos.findById(id).orElseThrow();
        db.owner = memo.owner;
        db.data = memo.data;
        return memos.saveAndFlush(db);
    }
    @RequestMapping(value = "/{id}", method = RequestMethod.DELETE)
    @PreAuthorize("hasPermission(#id, 'com.example.demo.memo.Memo', delete) or hasPermission(#id, 'com.example.demo.memo.Memo', admin)")
    public void delete(@PathVariable Long id) throws Exception {
        memos.deleteById(id);
        aclService.deleteAcl(new ObjectIdentityImpl(Memo.class, id), false);
    }

    @RequestMapping(value = "/{id}/perms/", method = RequestMethod.GET)
    @PreAuthorize(("hasPermission(#id, 'com.example.demo.memo.Memo', admin)"))
    public List<String> listPerms(@AuthenticationPrincipal(expression = "username") String username, @PathVariable Long id) {

        MutableAcl acl = (MutableAcl) aclService.readAclById(new ObjectIdentityImpl(Memo.class, id));
        Sid sid = new PrincipalSid(username);
        return acl.getEntries().stream()
                .filter(e -> !e.getSid().equals(sid))
                .collect(Collectors.groupingBy(el -> el.getSid()))
                .entrySet().stream()
                .map(e -> e.getKey().toString() + " " + e.getValue().stream().reduce(new CumulativePermission(), (p, ace) -> p.set(ace.getPermission()), (a,b) -> a.set(b)).getPattern())
                .toList();
    }

    @RequestMapping(value = "/{id}/perms/", method=RequestMethod.POST)
    @PreAuthorize("hasPermission(#id, 'com.example.demo.memo.Memo', admin)")
    public String createPerm(@AuthenticationPrincipal(expression = "username") String me, @PathVariable Long id, @RequestBody String them) throws Exception {
        if (me.equals(them)) throw new Exception("Can not change permissions for admin");
        return addPermission(them, id, BasePermission.READ).toString();
    }

    @RequestMapping(value = "/{id}/perms/", method=RequestMethod.DELETE)
    @PreAuthorize("hasPermission(#id, 'com.example.demo.memo.Memo', admin)")
    public void deletePerm(@AuthenticationPrincipal(expression = "username") String me, @PathVariable Long id, @RequestBody String user) throws Exception {
        if (me.equals(user)) throw new Exception ("Can not change permission for admin.");
        removePermissions(user, id);
    }

    private MutableAcl addPermission(String username, Long id, Permission permission) {
        // Prepare the information we'd like in our access control entry (ACE)
        ObjectIdentity oi = new ObjectIdentityImpl(Memo.class, id);
        Sid sid = new PrincipalSid(username);

        // Create or update the relevant ACL
        MutableAcl acl = null;
        try {
            acl = (MutableAcl) aclService.readAclById(oi);
        } catch (NotFoundException nfe) {
            acl = aclService.createAcl(oi);
        }

        // Now grant some permissions via an access control entry (ACE)
        acl.insertAce(acl.getEntries().size(), permission, sid, true);
        return aclService.updateAcl(acl);
    }

    private void removePermissions(String username, Long id) {
        Sid sid = new PrincipalSid(username);
        ObjectIdentity oid = new ObjectIdentityImpl(Memo.class, id);
        MutableAcl acl = (MutableAcl) aclService.readAclById(oid);
        List<Integer> toDelete = new ArrayList<>();
        int idx = 0;
        for (AccessControlEntry ace : acl.getEntries()) {
            if (ace.getSid().equals(sid)) {
               toDelete.add(idx);
            }
            idx++;
        }
        ListIterator<Integer> ids = toDelete.listIterator();
        while (ids.hasPrevious()) {
            acl.deleteAce(ids.previous());
        }

        aclService.updateAcl(acl);
    }

}


